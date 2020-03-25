using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace EtrendKeszito
{
    enum EtrendAdatTipus
    {
        EtrendAdat,
        Felhasznalo,
        Mertekegyseg,
        Penznem,
        Tapanyag,
        Etel,
        Ital,
        Menu,
        Koktel,
        EtkezesFeltetel,
        EtrendFeltetel,
        EtrendElelmiszer,
        EtrendIdopont
    }

    static class ABKezelo
    {
        static SqlConnection connection = new SqlConnection();
        static SqlCommand command = new SqlCommand();

        public static void Csatlakozas(string connString)
        {
            try
            {
                connection.ConnectionString = connString;
                connection.Open();
                command.Connection = connection;
            }
            catch (Exception e)
            {
                throw new ABKivetel("The connection is wrong", e);
            }
        }

        public static void KapcsolatBontas()
        {
            try
            {
                connection.Close();
                command.Dispose();
            }
            catch (Exception e)
            {
                throw new ABKivetel("Closing of the connection is wrong!", e);
            }
        }

        public static void setCurrentUser(Felhasznalo f)
        {
            SqlTransaction transaction = null;
            try
            {
                command.Parameters.Clear();
                transaction = connection.BeginTransaction("BeszurasStart");
                command.Transaction = transaction;

                command.CommandText =
                    "IF NOT EXISTS (SELECT * FROM [JelenlegiFelhasznaloNevHash]) BEGIN INSERT INTO [JelenlegiFelhasznaloNevHash] VALUES (@v1); END;";
                command.Parameters.AddWithValue("@v1", f.FelhasznaloNevHash);
                command.ExecuteNonQuery();

                command.CommandText =
                    "UPDATE [JelenlegiFelhasznaloNevHash] SET [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new ABKivetel("The insertion of the username is unsuccessful!", e);
            }
        }

        public static string GetCurrentUser()
        {
            try
            {
                command.Parameters.Clear();
                command.CommandText = "SELECT *FROM [JelenlegiFelhasznaloNevHash];";
                string str = null;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        str = reader["FelhasznaloNevHash"].ToString();
                    }
                    reader.Close();
                }

                return str;
            }
            catch (Exception e)
            {
                throw new ABKivetel("The search is unsuccessful!", e);
            }
        }

        public static int CountUsers(string felhasznaloNevHash)
        {
            try
            {
                command.Parameters.Clear();
                command.CommandText =
                    "SELECT COUNT(*) FROM [EtrendAdat] WHERE [EtrendAdat].[FelhasznaloNevHash] = @v1;";
                command.Parameters.AddWithValue("@v1", felhasznaloNevHash);
                return (int)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw new ABKivetel("The count is unsuccessful!", e);
            }
        }

        public static bool SikeresBelepes(Felhasznalo f)
        {
            try
            {
                command.Parameters.Clear();
                command.CommandText =
                    "SELECT COUNT(*) FROM [Felhasznalo] WHERE [Felhasznalo].[FelhasznaloNevHash] = @v1 AND [Felhasznalo].[JelszoHash]=@v2;";
                command.Parameters.AddWithValue("@v1", f.FelhasznaloNevHash);
                command.Parameters.AddWithValue("@v2", f.JelszoHash);
                return (int)command.ExecuteScalar() == 1;
            }
            catch (Exception e)
            {
                throw new ABKivetel("The count is unsuccessful!", e);
            }
        }


        public static string getJelszoHash(string felhasznaloNevHash)
        {
            try
            {
                command.Parameters.Clear();
                command.CommandText =
                    "SELECT [jelszoEmlekeztetoOTP] FROM [Felhasznalo] WHERE [Felhasznalo].[FelhasznaloNevHash] = @v1;";
                command.Parameters.AddWithValue("@v1", felhasznaloNevHash);
                string jelszoHash = null;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        jelszoHash = reader["jelszoEmlekeztetoOTP"].ToString();
                    }
                }
                return jelszoHash;
            }
            catch (Exception e)
            {
                throw new ABKivetel("The search for password is unsuccessful!", e);
            }
        }

        public static List<EtrendAdat> CommandProcess(List<EtrendAdat> etrendadatok, List<Elelmiszer> elelmiszerek)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    EtrendAdatTipus t = EtrendAdatTipus.EtrendAdat;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        int v = -1;
                        if (reader.GetName(i).ToLower() == "ElelmiszerTipus".ToLower())
                        {
                            v = Convert.ToInt32(reader.GetValue(i).ToString());
                        }
                        if (reader.GetName(i).ToLower() == "JelszoHash".ToLower()) t = EtrendAdatTipus.Felhasznalo;
                        else if (reader.GetName(i).ToLower() == "Valtoszam".ToLower()) t = EtrendAdatTipus.Mertekegyseg;
                        else if (reader.GetName(i).ToLower() == "PenzKod".ToLower()) t = EtrendAdatTipus.Penznem;
                        else if (reader.GetName(i).ToLower() == "NapiMinBevitel".ToLower()) t = EtrendAdatTipus.Tapanyag;
                        else if (reader.GetName(i).ToLower() == "ElelmiszerTipus".ToLower() && v == 0)
                            t = EtrendAdatTipus.Etel;
                        else if (reader.GetName(i).ToLower() == "ElelmiszerTipus".ToLower() && v == 1)
                            t = EtrendAdatTipus.Ital;
                        else if (reader.GetName(i).ToLower() == "ElelmiszerTipus".ToLower() && v == 2)
                            t = EtrendAdatTipus.Menu;
                        else if (reader.GetName(i).ToLower() == "ElelmiszerTipus".ToLower() && v == 3)
                            t = EtrendAdatTipus.Koktel;
                        else if (reader.GetName(i).ToLower() == "Eltipus2".ToLower()) t = EtrendAdatTipus.EtkezesFeltetel;
                        else if (reader.GetName(i).ToLower() == "Datum1".ToLower()) t = EtrendAdatTipus.EtrendFeltetel;
                        else if (reader.GetName(i).ToLower() == "Datum".ToLower()) t = EtrendAdatTipus.EtrendElelmiszer;
                        else if (reader.GetName(i).ToLower() == "ReggeliIdopont".ToLower()) t = EtrendAdatTipus.EtrendIdopont;
                    }

                    EtrendAdat e = null;
                    switch (t)
                    {
                        case EtrendAdatTipus.Felhasznalo:
                            e = new Felhasznalo(reader["FelhasznaloNevHash"].ToString(),
                                reader["JelszoHash"].ToString(),
                                reader["JelszoEmlekeztetoOTP"].ToString());
                            break;
                        case EtrendAdatTipus.Mertekegyseg:
                            e = new Mertekegyseg(reader["FelhasznaloNevHash"].ToString(),
                                reader["Megnevezes"].ToString(),
                                (MertekegysegFajta)Convert.ToByte(reader["Mertek"]),
                                Convert.ToDouble(reader["Valtoszam"]),
                                Convert.ToBoolean(reader["Hasznalhato"]),
                                Convert.ToBoolean(reader["Torolheto"]));
                            break;
                        case EtrendAdatTipus.Penznem:
                            e = new Penznem(reader["FelhasznaloNevHash"].ToString(),
                                reader["Megnevezes"].ToString(),
                                reader["PenzKod"].ToString(),
                                Convert.ToDouble(reader["Arfolyam"]),
                                Convert.ToByte(reader["TizedesekSzama"]),
                                Convert.ToBoolean(reader["Hasznalhato"]),
                                Convert.ToBoolean(reader["Torolheto"]));
                            break;
                        case EtrendAdatTipus.Tapanyag:
                            e = new Tapanyag(reader["FelhasznaloNevHash"].ToString(),
                                reader["Megnevezes"].ToString(),
                                null,
                                Convert.ToDouble(reader["NapiMinBevitel"]),
                                Convert.ToDouble(reader["NapiMaxBevitel"]),
                                Convert.ToBoolean(reader["NapiMax"]),
                                Convert.ToBoolean(reader["Hasznalhato"]));
                            break;
                        case EtrendAdatTipus.Etel:
                            e = new Etel(
                                reader["FelhasznaloNevHash"].ToString(),
                                reader["Megnevezes"].ToString(),
                                Convert.ToByte(reader["Orom"]),
                                null,
                                0,
                                Convert.ToBoolean(reader["EgysegTobbszorose"]),
                                new List<bool>
                                {
                                    Convert.ToBoolean(reader["ReggelinelFogyaszthato"]),
                                    Convert.ToBoolean(reader["EbednelFogyaszthato"]),
                                    Convert.ToBoolean(reader["VacsoranalFogyaszthato"])
                                },
                                Convert.ToBoolean(reader["Valtozatossag"]),
                                Convert.ToBoolean(reader["Hasznalhato"]),
                                new Dictionary<Tapanyag, double>(),
                                1, //ennek pozitivnak kell lennie, majd úgyis átállitjuk.
                                0,
                                0,
                                false,
                                null);
                            break;
                        case EtrendAdatTipus.Ital:
                            e = new Ital(
                                reader["FelhasznaloNevHash"].ToString(),
                                reader["Megnevezes"].ToString(),
                                Convert.ToByte(reader["Orom"]),
                                null,
                                0,
                                Convert.ToBoolean(reader["EgysegTobbszorose"]),
                                new List<bool>
                                {
                                    Convert.ToBoolean(reader["ReggelinelFogyaszthato"]),
                                    Convert.ToBoolean(reader["EbednelFogyaszthato"]),
                                    Convert.ToBoolean(reader["VacsoranalFogyaszthato"])
                                },
                                Convert.ToBoolean(reader["Valtozatossag"]),
                                Convert.ToBoolean(reader["Hasznalhato"]),
                                new Dictionary<Tapanyag, double>(),
                                1, //ennek pozitivnak kell lennie, majd úgyis átállitjuk.
                                0,
                                0,
                                false,
                                null);
                            break;
                        case EtrendAdatTipus.Menu:
                            e = new Menu(
                                reader["FelhasznaloNevHash"].ToString(),
                                reader["Megnevezes"].ToString(),
                                Convert.ToByte(reader["Orom"]),
                                null,
                                0,
                                Convert.ToBoolean(reader["EgysegTobbszorose"]),
                                new List<bool>
                                {
                                    Convert.ToBoolean(reader["ReggelinelFogyaszthato"]),
                                    Convert.ToBoolean(reader["EbednelFogyaszthato"]),
                                    Convert.ToBoolean(reader["VacsoranalFogyaszthato"])
                                },
                                Convert.ToBoolean(reader["Valtozatossag"]),
                                Convert.ToBoolean(reader["Hasznalhato"]),
                                new Dictionary<Tapanyag, double>(),
                                Convert.ToByte(reader["MaxDarab"]),
                                1,
                                null,
                                1,
                                null,
                                new Dictionary<Elelmiszer, double>(),
                                Convert.ToBoolean(reader["Arszamitas"]));
                            break;
                        case EtrendAdatTipus.EtkezesFeltetel:
                            e = new EtkezesFeltetel(
                                reader["FelhasznaloNevHash"].ToString(),
                                (ElelmiszerTipus2)Convert.ToByte(reader["Eltipus2"]),
                                (EtkezesTipus2)Convert.ToByte(reader["Ettipus2"]),
                                (Szamlalo)Convert.ToByte(reader["Szamlalo"]),
                                Convert.ToDouble(reader["Minval"]),
                                Convert.ToBoolean(reader["MaxvalE"]),
                                Convert.ToDouble(reader["Maxval"]));
                            break;
                        case EtrendAdatTipus.EtrendFeltetel:
                            e = new EtrendFeltetel(
                                reader["FelhasznaloNevHash"].ToString(),
                                Convert.ToDateTime(reader["Datum1"]),
                                (EtkezesTipus)Convert.ToByte(reader["Etkezes1"]),
                                Convert.ToDateTime(reader["Datum2"]),
                                (EtkezesTipus)Convert.ToByte(reader["Etkezes2"]),
                                Convert.ToBoolean(reader["Koltsegmin"]),
                                Convert.ToBoolean(reader["Orommax"]),
                                Convert.ToDouble(reader["Maxpenz"]),
                                null,
                                Convert.ToBoolean(reader["Solverelrejt"]),
                                Convert.ToBoolean(reader["Folytonosmodell"]),
                                Convert.ToInt32(reader["Numvaltozatossag"]),
                                Convert.ToInt32(reader["Maxfutasiido"]),
                                Convert.ToBoolean(reader["NaptarbaMent"]));
                            break;
                        case EtrendAdatTipus.EtrendElelmiszer:
                            e = new EtrendElelmiszer(
                                reader["FelhasznaloNevHash"].ToString(),
                                Convert.ToDateTime(reader["Datum"]),
                                (EtkezesTipus)Convert.ToByte(reader["Etkezes"]),
                                    elelmiszerek.Where(x => x.Megnevezes == reader["ElelmiszerMegnevezes"].ToString()).ToList().First(),
                                Convert.ToDouble(reader["Val"]),
                                Convert.ToDouble(reader["Tomeg"]),
                                null,
                                Convert.ToDouble(reader["Urmertek"]),
                                null);
                            break;
                        case EtrendAdatTipus.EtrendIdopont:
                            e = new EtrendIdopont(
                                reader["FelhasznaloNevHash"].ToString(),
                                new List<DateTime>{
                                Convert.ToDateTime(reader["ReggeliIdopont"]),
                                Convert.ToDateTime(reader["EbedIdopont"]),
                                Convert.ToDateTime(reader["VacsoraIdopont"])});
                            break;
                    }
                    if (e != null) etrendadatok.Add(e);
                }
                reader.Close();
            }
            return etrendadatok;
        }

        public static List<EtrendAdat> Kiolvasas()
        {
            try
            {
                List<EtrendAdat> etrendadatok = new List<EtrendAdat>();

                string fnevhash = GetCurrentUser();
                //mértékegységek hozzáadása....
                command.Parameters.Clear();
                command.CommandText =
                    "SELECT * FROM [Mertekegyseg] WHERE [Mertekegyseg].[FelhasznaloNevHash]=@v1;";
                command.Parameters.AddWithValue("@v1", fnevhash);
                etrendadatok = CommandProcess(etrendadatok, null);

                //pénznemek hozzáadása
                command.CommandText =
                    "SELECT * FROM [Penznem] WHERE [Penznem].[FelhasznaloNevHash]=@v1;";
                etrendadatok = CommandProcess(etrendadatok, null);

                //tápanyagok hozzáadása
                command.CommandText =
                    "SELECT * FROM [Tapanyag] WHERE [Tapanyag].[FelhasznaloNevHash]=@v1;";
                etrendadatok = CommandProcess(etrendadatok, null);

                //étkezésfeltételek hozzáadása
                command.CommandText =
                    "SELECT * FROM [EtkezesFeltetel] WHERE [EtkezesFeltetel].[FelhasznaloNevHash]=@v1;";
                etrendadatok = CommandProcess(etrendadatok, null);

                //étrendfeltételek hozzáadása
                command.CommandText =
                    "SELECT * FROM [EtrendFeltetel] WHERE [EtrendFeltetel].[FelhasznaloNevHash]=@v1;";
                etrendadatok = CommandProcess(etrendadatok, null);

                //étrendidőpontok hozzáadása
                command.CommandText =
                    "SELECT * FROM [EtrendIdopont] WHERE [EtrendIdopont].[FelhasznaloNevHash]=@v1;";
                etrendadatok = CommandProcess(etrendadatok, null);

                List<Elelmiszer> elelmiszerek = new List<Elelmiszer>();

                var mertekegysegek = etrendadatok.Where(x => x is Mertekegyseg).ToList();
                var penznemek = etrendadatok.Where(x => x is Penznem).ToList();
                var tapanyagok = etrendadatok.Where(x => x is Tapanyag).ToList();


                foreach (EtrendAdat item in etrendadatok)
                {
                    if (item is Tapanyag)
                    {
                        command.Parameters.Clear();
                        command.CommandText =
                            "SELECT * FROM [TapanyagMertekegyseg] WHERE [TapanyagMertekegyseg].[FelhasznaloNevHash]=@v1 AND [TapanyagMertekegyseg].[TapanyagMegnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", item.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (item as Tapanyag).Megnevezes);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string str = reader["MertekegysegMegnevezes"].ToString();
                                Mertekegyseg m = (Mertekegyseg)mertekegysegek.Where(x => (x as Mertekegyseg).Megnevezes == str).First();
                                (item as Tapanyag).Mertek = m;
                            }
                        }
                    }
                }

                //ételek hozzáadása
                command.Parameters.Clear();
                command.CommandText =
                    "SELECT * FROM [Etel] WHERE [Etel].[FelhasznaloNevHash]=@v1;";
                command.Parameters.AddWithValue("@v1", fnevhash);
                etrendadatok = CommandProcess(etrendadatok, null);

                //italok hozzáadása
                command.Parameters.Clear();
                command.CommandText =
                    "SELECT * FROM [Ital] WHERE [Ital].[FelhasznaloNevHash]=@v1;";
                command.Parameters.AddWithValue("@v1", fnevhash);
                etrendadatok = CommandProcess(etrendadatok, null);

                //menük hozzáadása
                command.Parameters.Clear();
                command.CommandText =
                    "SELECT * FROM [Menu] WHERE [Menu].[FelhasznaloNevHash]=@v1;";
                command.Parameters.AddWithValue("@v1", fnevhash);
                etrendadatok = CommandProcess(etrendadatok, null);

                foreach (EtrendAdat item in etrendadatok)
                {
                    if (item is Elelmiszer)
                    {
                        command.Parameters.Clear();
                        command.CommandText =
                            "SELECT * FROM [ElelmiszerPenznem] WHERE [ElelmiszerPenznem].[FelhasznaloNevHash]=@v1 AND [ElelmiszerPenznem].[ElelmiszerMegnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", item.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (item as Elelmiszer).Megnevezes);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string str = reader["PenznemMegnevezes"].ToString();
                                double ar = Convert.ToDouble(reader["Ar"].ToString());
                                Penznem p = (Penznem)penznemek.Where(x => (x as Penznem).Megnevezes == str).First();
                                (item as Elelmiszer).Penz = p;
                                (item as Elelmiszer).Ar = ar;
                            }
                        }

                        command.CommandText =
                            "SELECT * FROM [ElelmiszerTapanyag] WHERE [ElelmiszerTapanyag].[FelhasznaloNevHash]=@v1 AND [ElelmiszerTapanyag].[ElelmiszerMegnevezes]=@v2;";
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string str = reader["TapanyagMegnevezes"].ToString();
                                double ar = Convert.ToDouble(reader["Mennyiseg"].ToString());
                                Tapanyag t = (Tapanyag)tapanyagok.Where(x => (x as Tapanyag).Megnevezes == str).First();
                                (item as Elelmiszer).TapanyagTartalom.Add(t, ar);
                            }
                        }
                    }
                }


                foreach (EtrendAdat item in etrendadatok)
                {
                    if (item is Etel)
                    {
                        command.Parameters.Clear();
                        command.CommandText =
                            "SELECT * FROM [ElelmiszerTomeg] WHERE [ElelmiszerTomeg].[FelhasznaloNevHash]=@v1 AND [ElelmiszerTomeg].[ElelmiszerMegnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", item.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (item as Etel).Megnevezes);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string str = reader["MertekegysegMegnevezes"].ToString();
                                Mertekegyseg m = (Mertekegyseg)mertekegysegek.Where(x => (x as Mertekegyseg).Megnevezes == str).First();
                                (item as Etel).TomegMertek = m;
                                (item as Etel).EgysegTomegMennyiseg = Convert.ToDouble(reader["Tomeg"].ToString());
                                (item as Etel).MinTomeg = Convert.ToDouble(reader["MinTomeg"]);
                                (item as Etel).MaxTomeg = Convert.ToDouble(reader["MaxTomeg"]);
                                (item as Etel).MaxTomegE = Convert.ToBoolean(reader["MaxTomegE"]);
                            }
                        }
                    }
                    if (item is Ital)
                    {
                        command.Parameters.Clear();
                        command.CommandText =
                            "SELECT * FROM [ElelmiszerUrtartalom] WHERE [ElelmiszerUrtartalom].[FelhasznaloNevHash]=@v1 AND [ElelmiszerUrtartalom].[ElelmiszerMegnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", item.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (item as Ital).Megnevezes);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string str = reader["MertekegysegMegnevezes"].ToString();
                                Mertekegyseg m = (Mertekegyseg)mertekegysegek.Where(x => (x as Mertekegyseg).Megnevezes == str).First();
                                (item as Ital).Urmertek = m;
                                (item as Ital).EgysegUrTartalomMennyiseg = Convert.ToDouble(reader["Urtartalom"].ToString());
                                (item as Ital).MinUrTartalom = Convert.ToDouble(reader["MinUrtartalom"]);
                                (item as Ital).MaxUrTartalom = Convert.ToDouble(reader["MaxUrtartalom"]);
                                (item as Ital).MaxUrTartalomE = Convert.ToBoolean(reader["MaxUrtartalomE"]);
                            }
                        }
                    }
                }

                elelmiszerek = new List<Elelmiszer>();
                foreach (EtrendAdat item in etrendadatok)
                {
                    if (item is Elelmiszer) elelmiszerek.Add((Elelmiszer)item);
                }

                foreach (EtrendAdat item in etrendadatok)
                {
                    if (item is Menu)
                    {
                        command.Parameters.Clear();
                        command.CommandText =
                            "SELECT * FROM [ElelmiszerTomeg] WHERE [ElelmiszerTomeg].[FelhasznaloNevHash]=@v1 AND [ElelmiszerTomeg].[ElelmiszerMegnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", item.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (item as Menu).Megnevezes);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string str = reader["MertekegysegMegnevezes"].ToString();
                                Mertekegyseg m = (Mertekegyseg)mertekegysegek.Where(x => (x as Mertekegyseg).Megnevezes == str).First();
                                (item as Menu).TomegMertek = m;
                                (item as Menu).EgysegTomegMennyiseg = Convert.ToDouble(reader["Tomeg"].ToString());
                            }
                        }

                        command.Parameters.Clear();
                        command.CommandText =
                            "SELECT * FROM [ElelmiszerUrtartalom] WHERE [ElelmiszerUrtartalom].[FelhasznaloNevHash]=@v1 AND [ElelmiszerUrtartalom].[ElelmiszerMegnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", item.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (item as Menu).Megnevezes);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string str = reader["MertekegysegMegnevezes"].ToString();
                                Mertekegyseg m = (Mertekegyseg)mertekegysegek.Where(x => (x as Mertekegyseg).Megnevezes == str).First();
                                (item as Menu).Urmertek = m;
                                (item as Menu).EgysegUrTartalomMennyiseg = Convert.ToDouble(reader["Urtartalom"].ToString());
                            }
                        }

                        command.CommandText =
                            "SELECT * FROM [ElelmiszerElelmiszer] WHERE [ElelmiszerElelmiszer].[FelhasznaloNevHash]=@v1 AND [ElelmiszerElelmiszer].[Megnevezes]=@v2;";
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string str = reader["Osszetevo"].ToString();
                                double m = Convert.ToDouble(reader["Mennyiseg"].ToString());
                                Elelmiszer e = (Elelmiszer)elelmiszerek.Where(x => (x as Elelmiszer).Megnevezes == str).First();
                                (item as Menu).Osszetevo.Add(e, m);
                            }
                        }
                    }
                }

                elelmiszerek = new List<Elelmiszer>();
                foreach (EtrendAdat item in etrendadatok)
                {
                    if (item is Elelmiszer) elelmiszerek.Add((Elelmiszer)item);
                }
                //étrendélelmiszerek hozzáadása
                command.CommandText =
                    "SELECT * FROM [EtrendElelmiszer] WHERE [EtrendElelmiszer].[FelhasznaloNevHash]=@v1;";
                etrendadatok = CommandProcess(etrendadatok, elelmiszerek);



                foreach (EtrendAdat item in etrendadatok)
                {
                    if (item is EtrendFeltetel)
                    {
                        command.Parameters.Clear();
                        command.CommandText =
                            "SELECT * FROM [EtrendFeltetel] WHERE [EtrendFeltetel].[FelhasznaloNevHash]=@v1;";
                        command.Parameters.AddWithValue("@v1", item.FelhasznaloNevHash);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string str = reader["PenznemMegnevezes"].ToString();
                                Penznem p = (Penznem)penznemek.Where(x => (x as Penznem).Megnevezes == str).First();
                                (item as EtrendFeltetel).Penz = p;
                            }
                        }
                    }
                }

                foreach (EtrendAdat item in etrendadatok)
                {
                    if (item is EtrendElelmiszer)
                    {
                        command.Parameters.Clear();
                        command.CommandText =
                            "SELECT * FROM [EtrendElelmiszer] WHERE [EtrendElelmiszer].[FelhasznaloNevHash]=@v1 AND [EtrendElelmiszer].[Datum]=@v2 AND [EtrendElelmiszer].[Etkezes]=@v3 AND [EtrendElelmiszer].[ElelmiszerMegnevezes]=@v4;";
                        command.Parameters.AddWithValue("@v1", item.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (item as EtrendElelmiszer).Datum);
                        command.Parameters.AddWithValue("@v3", Convert.ToByte(((int)(item as EtrendElelmiszer).Etkezes)));
                        command.Parameters.AddWithValue("@v4", (item as EtrendElelmiszer).Elelmiszer.Megnevezes);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string str;
                                Mertekegyseg m;

                                str = reader["TomegmertekegysegMegnevezes"].ToString();
                                m = (Mertekegyseg)mertekegysegek.Where(x => (x as Mertekegyseg).Megnevezes == str).First();
                                (item as EtrendElelmiszer).Tomegmertekegyseg = m;

                                str = reader["UrmertekegysegMegnevezes"].ToString();
                                m = (Mertekegyseg)mertekegysegek.Where(x => (x as Mertekegyseg).Megnevezes == str).First();
                                (item as EtrendElelmiszer).Urmertekegyseg = m;
                            }
                        }
                    }
                }

                return etrendadatok;
            }
            catch (Exception e)
            {
                throw new ABKivetel("The read is unsuccessful!", e);
            }
        }

        public static void BeszurTapanyagElelmiszerekbe(Tapanyag t, List<Elelmiszer> elelmiszerek = null)
        {
            SqlTransaction transaction = null;
            try
            {
                command.Parameters.Clear();
                transaction = connection.BeginTransaction("BeszurasStart");
                command.Transaction = transaction;
                string fnevhash = GetCurrentUser();

                if (elelmiszerek == null)
                {
                    elelmiszerek = new List<Elelmiszer>();
                    foreach (Elelmiszer item in ABKezelo.Kiolvasas().Where(x => x is Elelmiszer).ToList())
                    {
                        elelmiszerek.Add(item);
                    }
                }

                foreach (Elelmiszer item in elelmiszerek)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [ElelmiszerTapanyag] VALUES(@v1,@v2,@v3,@v4);";
                    command.Parameters.AddWithValue("@v1", fnevhash);
                    command.Parameters.AddWithValue("@v2", item.Megnevezes);
                    command.Parameters.AddWithValue("@v3", t.Megnevezes);
                    command.Parameters.AddWithValue("@v4", (float)0);
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new ABKivetel("The insertion is unsuccessful!", e);
            }
        }

        public static void BeszurElelmiszerElelmiszerekbe(Elelmiszer e)
        {
            SqlTransaction transaction = null;
            try
            {
                command.Parameters.Clear();
                transaction = connection.BeginTransaction("BeszurasStart");
                command.Transaction = transaction;
                string fnevhash = GetCurrentUser();

                foreach (Elelmiszer item in ABKezelo.Kiolvasas().Where(x => x is Elelmiszer).ToList())
                {
                    if (item is Menu) continue;//menü élelmiszere nem lehet menü
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [ElelmiszerElelmiszer] VALUES(@v1,@v2,@v3,@v4);";
                    command.Parameters.AddWithValue("@v1", fnevhash);
                    command.Parameters.AddWithValue("@v2", item.Megnevezes);
                    command.Parameters.AddWithValue("@v3", e.Megnevezes);
                    command.Parameters.AddWithValue("@v4", (float)0);
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ABKivetel("The insertion is unsuccessful!", ex);
            }
        }

        public static void Beszuras(EtrendAdat uj)
        {
            SqlTransaction transaction = null;
            try
            {
                command.Parameters.Clear();
                transaction = connection.BeginTransaction("BeszurasStart");
                command.Transaction = transaction;

                if (uj is Felhasznalo)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [EtrendAdat] VALUES(@v1);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [Felhasznalo] VALUES(@v1,@v2,@v3);";
                    command.Parameters.AddWithValue("@v1", (uj as Felhasznalo).FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Felhasznalo).JelszoHash);
                    command.Parameters.AddWithValue("@v3", (uj as Felhasznalo).JelszoEmlekeztetoOTP);
                    command.ExecuteNonQuery();
                }
                else if (uj is Mertekegyseg)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [Mertekegyseg] VALUES(@v1,@v2,@v3,@v4,@v5,@v6);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Mertekegyseg).Megnevezes);
                    command.Parameters.AddWithValue("@v3", Convert.ToByte((uj as Mertekegyseg).Mertek));
                    command.Parameters.AddWithValue("@v4", (float)(uj as Mertekegyseg).Valtoszam);
                    command.Parameters.AddWithValue("@v5", Convert.ToByte((uj as Mertekegyseg).Hasznalhato));
                    command.Parameters.AddWithValue("@v6", Convert.ToByte((uj as Mertekegyseg).Torolheto));
                    command.ExecuteNonQuery();
                }
                else if (uj is Penznem)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [Penznem] VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Penznem).Megnevezes);
                    command.Parameters.AddWithValue("@v3", (uj as Penznem).PenzKod);
                    command.Parameters.AddWithValue("@v4", (float)(uj as Penznem).Arfolyam);
                    command.Parameters.AddWithValue("@v5", Convert.ToByte((uj as Penznem).TizedesekSzama));
                    command.Parameters.AddWithValue("@v6", Convert.ToByte((uj as Penznem).Hasznalhato));
                    command.Parameters.AddWithValue("@v7", Convert.ToByte((uj as Penznem).Torolheto));
                    command.ExecuteNonQuery();
                }
                else if (uj is Tapanyag)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [Tapanyag] VALUES(@v1,@v2,@v3,@v4,@v5,@v6);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Tapanyag).Megnevezes);
                    command.Parameters.AddWithValue("@v3", (float)(uj as Tapanyag).NapiMinBevitel);
                    command.Parameters.AddWithValue("@v4", (float)(uj as Tapanyag).NapiMaxBevitel);
                    command.Parameters.AddWithValue("@v5", Convert.ToByte((uj as Tapanyag).NapiMax));
                    command.Parameters.AddWithValue("@v6", Convert.ToByte((uj as Tapanyag).Hasznalhato));
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [TapanyagMertekegyseg] VALUES(@v1,@v2,@v3);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Tapanyag).Megnevezes);
                    command.Parameters.AddWithValue("@v3", (uj as Tapanyag).Mertek.Megnevezes);
                    command.ExecuteNonQuery();
                }
                else if (uj is EtkezesFeltetel)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [EtkezesFeltetel] VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", Convert.ToByte((uj as EtkezesFeltetel).Eltipus2));
                    command.Parameters.AddWithValue("@v3", Convert.ToByte((uj as EtkezesFeltetel).Ettipus2));
                    command.Parameters.AddWithValue("@v4", Convert.ToByte((uj as EtkezesFeltetel).Szamlalo));
                    command.Parameters.AddWithValue("@v5", (float)(uj as EtkezesFeltetel).Minval);
                    command.Parameters.AddWithValue("@v6", Convert.ToByte((uj as EtkezesFeltetel).MaxvalE));
                    command.Parameters.AddWithValue("@v7", (float)(uj as EtkezesFeltetel).Maxval);
                    command.ExecuteNonQuery();
                }
                else if (uj is EtrendFeltetel)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [EtrendFeltetel] VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7,@v8,@v9,@v10,@v11,@v12,@v13,@v14);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", Convert.ToDateTime((uj as EtrendFeltetel).Datum1));
                    command.Parameters.AddWithValue("@v3", (byte)(uj as EtrendFeltetel).Etkezes1);
                    command.Parameters.AddWithValue("@v4", Convert.ToDateTime((uj as EtrendFeltetel).Datum2));
                    command.Parameters.AddWithValue("@v5", (byte)(uj as EtrendFeltetel).Etkezes2);
                    command.Parameters.AddWithValue("@v6", Convert.ToByte((uj as EtrendFeltetel).Koltsegmin));
                    command.Parameters.AddWithValue("@v7", Convert.ToByte((uj as EtrendFeltetel).Orommax));
                    command.Parameters.AddWithValue("@v8", (float)(uj as EtrendFeltetel).Maxpenz);
                    command.Parameters.AddWithValue("@v9", (uj as EtrendFeltetel).Penz.Megnevezes);
                    command.Parameters.AddWithValue("@v10", Convert.ToByte((uj as EtrendFeltetel).Solverelrejt));
                    command.Parameters.AddWithValue("@v11", Convert.ToByte((uj as EtrendFeltetel).Folytonosmodell));
                    command.Parameters.AddWithValue("@v12", Convert.ToInt32((uj as EtrendFeltetel).Numvaltozatossag));
                    command.Parameters.AddWithValue("@v13", Convert.ToInt32((uj as EtrendFeltetel).Maxfutasiido));
                    command.Parameters.AddWithValue("@v14", Convert.ToByte((uj as EtrendFeltetel).NaptarbaMent));
                    command.ExecuteNonQuery();
                }
                else if (uj is EtrendElelmiszer)
                {
                    command.Parameters.Clear();
                    command.CommandText =
                        "INSERT INTO [EtrendElelmiszer] VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7,@v8,@v9);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", Convert.ToDateTime((uj as EtrendElelmiszer).Datum));
                    command.Parameters.AddWithValue("@v3", Convert.ToByte((uj as EtrendElelmiszer).Etkezes));
                    command.Parameters.AddWithValue("@v4", (uj as EtrendElelmiszer).Elelmiszer.Megnevezes);
                    command.Parameters.AddWithValue("@v5", (float)(uj as EtrendElelmiszer).Val);

                    command.Parameters.AddWithValue("@v6", (float)(uj as EtrendElelmiszer).Tomeg);
                    command.Parameters.AddWithValue("@v7", (uj as EtrendElelmiszer).Tomegmertekegyseg.Megnevezes);
                    command.Parameters.AddWithValue("@v8", (float)(uj as EtrendElelmiszer).Urmertek);
                    command.Parameters.AddWithValue("@v9", (uj as EtrendElelmiszer).Urmertekegyseg.Megnevezes);
                    command.ExecuteNonQuery();
                }
                else if (uj is EtrendIdopont)
                {
                    command.Parameters.Clear();
                    command.CommandText =
                        "INSERT INTO [EtrendIdopont] VALUES(@v1,@v2,@v3,@v4);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", Convert.ToDateTime((uj as EtrendIdopont).Datum[0]));
                    command.Parameters.AddWithValue("@v3", Convert.ToDateTime((uj as EtrendIdopont).Datum[1]));
                    command.Parameters.AddWithValue("@v4", Convert.ToDateTime((uj as EtrendIdopont).Datum[2]));
                    command.ExecuteNonQuery();
                }


                if (uj is Elelmiszer)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [Elelmiszer] VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7,@v9,@v9);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Elelmiszer).Megnevezes);
                    command.Parameters.AddWithValue("@v3", Convert.ToByte((uj as Elelmiszer).Orom));
                    command.Parameters.AddWithValue("@v4", Convert.ToByte((uj as Elelmiszer).EgysegTobbszorose));
                    command.Parameters.AddWithValue("@v5", Convert.ToByte((uj as Elelmiszer).Fogyaszthato[0]));
                    command.Parameters.AddWithValue("@v6", Convert.ToByte((uj as Elelmiszer).Fogyaszthato[1]));
                    command.Parameters.AddWithValue("@v7", Convert.ToByte((uj as Elelmiszer).Fogyaszthato[2]));
                    command.Parameters.AddWithValue("@v8", Convert.ToByte((uj as Elelmiszer).Valtozatossag));
                    command.Parameters.AddWithValue("@v9", Convert.ToByte((uj as Elelmiszer).Hasznalhato));
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [ElelmiszerPenznem] VALUES(@v1,@v2,@v3,@v4);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Elelmiszer).Megnevezes);
                    command.Parameters.AddWithValue("@v3", (uj as Elelmiszer).Penz.Megnevezes);
                    command.Parameters.AddWithValue("@v4", (float)((uj as Elelmiszer).Ar));
                    command.ExecuteNonQuery();

                    foreach (KeyValuePair<Tapanyag, Double> item in (uj as Elelmiszer).TapanyagTartalom)
                    {
                        string str = item.Key.Megnevezes;
                        double m = Convert.ToDouble(item.Value);

                        command.Parameters.Clear();
                        command.CommandText = "INSERT INTO [ElelmiszerTapanyag] VALUES(@v1,@v2,@v3,@v4);";
                        command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (uj as Elelmiszer).Megnevezes);
                        command.Parameters.AddWithValue("@v3", str);
                        command.Parameters.AddWithValue("@v4", (float)m);
                        command.ExecuteNonQuery();
                    }
                }

                if (uj is Etel)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [ElelmiszerTomeg] VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Etel).Megnevezes);
                    command.Parameters.AddWithValue("@v3", (uj as Etel).TomegMertek.Megnevezes);
                    command.Parameters.AddWithValue("@v4", (float)((uj as Etel).EgysegTomegMennyiseg));
                    command.Parameters.AddWithValue("@v5", (float)((uj as Etel).MinTomeg));
                    command.Parameters.AddWithValue("@v6", (float)((uj as Etel).MaxTomeg));
                    command.Parameters.AddWithValue("@v7", Convert.ToByte((uj as Etel).MaxTomegE));
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [Etel] VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7,@v8,@v9,@v10,@v11);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Etel).Megnevezes);
                    command.Parameters.AddWithValue("@v3", Convert.ToByte(0)); //ételhez 0 tartozik
                    command.Parameters.AddWithValue("@v4", Convert.ToByte((uj as Etel).Orom));
                    command.Parameters.AddWithValue("@v5", Convert.ToByte((uj as Etel).EgysegTobbszorose));
                    command.Parameters.AddWithValue("@v6", Convert.ToByte((uj as Etel).Fogyaszthato[0]));
                    command.Parameters.AddWithValue("@v7", Convert.ToByte((uj as Etel).Fogyaszthato[1]));
                    command.Parameters.AddWithValue("@v8", Convert.ToByte((uj as Etel).Fogyaszthato[2]));
                    command.Parameters.AddWithValue("@v9", Convert.ToByte((uj as Etel).Valtozatossag));
                    command.Parameters.AddWithValue("@v10", Convert.ToByte((uj as Etel).Hasznalhato));
                    command.Parameters.AddWithValue("@v11", (uj as Etel).TomegMertek.Megnevezes);

                    command.ExecuteNonQuery();
                }
                else if (uj is Ital)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [ElelmiszerUrtartalom] VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Ital).Megnevezes);
                    command.Parameters.AddWithValue("@v3", (uj as Ital).Urmertek.Megnevezes);
                    command.Parameters.AddWithValue("@v4", (float)((uj as Ital
                        ).EgysegUrTartalomMennyiseg));
                    command.Parameters.AddWithValue("@v5", (float)((uj as Ital).MinUrTartalom));
                    command.Parameters.AddWithValue("@v6", (float)((uj as Ital).MaxUrTartalom));
                    command.Parameters.AddWithValue("@v7", Convert.ToByte((uj as Ital).MaxUrTartalomE));
                    command.ExecuteNonQuery();


                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [Ital] VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7,@v8,@v9,@v10,@v11);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Ital).Megnevezes);
                    command.Parameters.AddWithValue("@v3", Convert.ToByte(1)); //italhoz 1 tartozik
                    command.Parameters.AddWithValue("@v4", Convert.ToByte((uj as Ital).Orom));
                    command.Parameters.AddWithValue("@v5", Convert.ToByte((uj as Ital
                        ).EgysegTobbszorose));
                    command.Parameters.AddWithValue("@v6", Convert.ToByte((uj as Ital).Fogyaszthato[0]));
                    command.Parameters.AddWithValue("@v7", Convert.ToByte((uj as Ital).Fogyaszthato[1]));
                    command.Parameters.AddWithValue("@v8", Convert.ToByte((uj as Ital).Fogyaszthato[2]));
                    command.Parameters.AddWithValue("@v9", Convert.ToByte((uj as Ital).Valtozatossag));
                    command.Parameters.AddWithValue("@v10", Convert.ToByte((uj as Ital).Hasznalhato));
                    command.Parameters.AddWithValue("@v11", (uj as Ital).Urmertek.Megnevezes);

                    command.ExecuteNonQuery();
                }

                if (uj is Menu)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [ElelmiszerTomeg] VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Menu).Megnevezes);
                    command.Parameters.AddWithValue("@v3", (uj as Menu).TomegMertek.Megnevezes);
                    command.Parameters.AddWithValue("@v4", (float)((uj as Menu).EgysegTomegMennyiseg));
                    command.Parameters.AddWithValue("@v5", (float)0);
                    command.Parameters.AddWithValue("@v6", (float)0);
                    command.Parameters.AddWithValue("@v7", (byte)0);
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [ElelmiszerUrtartalom] VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Menu).Megnevezes);
                    command.Parameters.AddWithValue("@v3", (uj as Menu).Urmertek.Megnevezes);
                    command.Parameters.AddWithValue("@v4", (float)((uj as Menu
                        ).EgysegUrTartalomMennyiseg));
                    command.Parameters.AddWithValue("@v5", (float)0);
                    command.Parameters.AddWithValue("@v6", (float)0);
                    command.Parameters.AddWithValue("@v7", (byte)0);
                    command.ExecuteNonQuery();


                    foreach (KeyValuePair<Elelmiszer, Double> item in (uj as Menu).Osszetevo)
                    {
                        if (item.Value > 0)
                        {

                            command.Parameters.Clear();
                            command.CommandText =
                                "INSERT INTO [ElelmiszerElelmiszer] VALUES(@v1,@v2,@v3,@v4);";
                            command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                            command.Parameters.AddWithValue("@v2", (uj as Menu).Megnevezes);
                            command.Parameters.AddWithValue("@v3", item.Key.Megnevezes);
                            command.Parameters.AddWithValue("@v4", (float)item.Value);
                            command.ExecuteNonQuery();
                        }
                    }

                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO [Menu] VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7,@v8,@v9,@v10,@v11,@v12,@v13,@v14);";
                    command.Parameters.AddWithValue("@v1", uj.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (uj as Menu).Megnevezes);
                    command.Parameters.AddWithValue("@v3", Convert.ToByte(2)); //menü 2-es tartozik!
                    command.Parameters.AddWithValue("@v4", Convert.ToByte((uj as Menu).Orom));
                    command.Parameters.AddWithValue("@v5", Convert.ToByte((uj as Menu).EgysegTobbszorose));
                    command.Parameters.AddWithValue("@v6", Convert.ToByte((uj as Menu).Fogyaszthato[0]));
                    command.Parameters.AddWithValue("@v7", Convert.ToByte((uj as Menu).Fogyaszthato[1]));
                    command.Parameters.AddWithValue("@v8", Convert.ToByte((uj as Menu).Fogyaszthato[2]));
                    command.Parameters.AddWithValue("@v9", Convert.ToByte((uj as Menu).Valtozatossag));
                    command.Parameters.AddWithValue("@v10", Convert.ToByte((uj as Menu).Hasznalhato));
                    command.Parameters.AddWithValue("@v11", (uj as Menu).TomegMertek.Megnevezes);
                    command.Parameters.AddWithValue("@v12", (uj as Menu).Urmertek.Megnevezes);
                    command.Parameters.AddWithValue("@v13", Convert.ToByte((uj as Menu).MaxDarab));
                    command.Parameters.AddWithValue("@v14", Convert.ToByte((uj as Menu).Arszamitas));
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new ABKivetel("The insertion is unsuccessful!", e);
            }
        }

        public static void Modositas(EtrendAdat modosit)
        {
            SqlTransaction transaction = null;
            try
            {
                command.Parameters.Clear();
                transaction = connection.BeginTransaction("Modositas");
                command.Transaction = transaction;

                if (modosit is Mertekegyseg)
                {
                    command.Parameters.Clear();
                    command.CommandText =
                        "UPDATE [Mertekegyseg] SET [Hasznalhato] = @v1 WHERE [FelhasznaloNevHash] = @v2 AND [Megnevezes]=@v3;";
                    command.Parameters.AddWithValue("@v1", Convert.ToByte((modosit as Mertekegyseg).Hasznalhato));
                    command.Parameters.AddWithValue("@v2", modosit.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v3", (modosit as Mertekegyseg).Megnevezes);
                    command.ExecuteNonQuery();
                }
                else if (modosit is Penznem)
                {
                    command.Parameters.Clear();
                    command.CommandText =
                        "UPDATE [Penznem] SET [Arfolyam]=@v1, [TizedesekSzama]=@v2, [Hasznalhato] = @v3 WHERE [FelhasznaloNevHash] = @v4 AND [Megnevezes]=@v5;";
                    command.Parameters.AddWithValue("@v1", Convert.ToSingle((modosit as Penznem).Arfolyam));
                    command.Parameters.AddWithValue("@v2", Convert.ToByte((modosit as Penznem).TizedesekSzama));
                    command.Parameters.AddWithValue("@v3", Convert.ToByte((modosit as Penznem).Hasznalhato));
                    command.Parameters.AddWithValue("@v4", modosit.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v5", (modosit as Penznem).Megnevezes);
                    command.ExecuteNonQuery();
                }
                else if (modosit is Tapanyag)
                {
                    command.Parameters.Clear();
                    command.CommandText =
                        "UPDATE [Tapanyag] SET [NapiMinBevitel]=@v1, [NapiMaxBevitel]=@v2, [NapiMax] = @v3, [Hasznalhato] = @v4 WHERE [FelhasznaloNevHash] = @v5 AND [Megnevezes]=@v6;";
                    command.Parameters.AddWithValue("@v1", Convert.ToSingle((modosit as Tapanyag).NapiMinBevitel));
                    command.Parameters.AddWithValue("@v2", Convert.ToSingle((modosit as Tapanyag).NapiMaxBevitel));
                    command.Parameters.AddWithValue("@v3", Convert.ToByte((modosit as Tapanyag).NapiMax));
                    command.Parameters.AddWithValue("@v4", Convert.ToByte((modosit as Tapanyag).Hasznalhato));
                    command.Parameters.AddWithValue("@v5", modosit.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v6", (modosit as Tapanyag).Megnevezes);
                    command.ExecuteNonQuery();
                }
                else if (modosit is EtkezesFeltetel)
                {
                    command.Parameters.Clear();
                    command.CommandText =
                        "UPDATE [EtkezesFeltetel] SET [Minval]=@v1, [MaxvalE]=@v2, [Maxval] = @v3 WHERE [FelhasznaloNevHash] = @v4 AND [Eltipus2]=@v5 AND [Ettipus2]=@v6 AND [Szamlalo]=@v7;";
                    command.Parameters.AddWithValue("@v1", Convert.ToSingle((modosit as EtkezesFeltetel).Minval));
                    command.Parameters.AddWithValue("@v2", Convert.ToByte((modosit as EtkezesFeltetel).MaxvalE));
                    command.Parameters.AddWithValue("@v3", Convert.ToSingle((modosit as EtkezesFeltetel).Maxval));
                    command.Parameters.AddWithValue("@v4", modosit.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v5", Convert.ToByte((modosit as EtkezesFeltetel).Eltipus2));
                    command.Parameters.AddWithValue("@v6", Convert.ToByte((modosit as EtkezesFeltetel).Ettipus2));
                    command.Parameters.AddWithValue("@v7", Convert.ToByte((modosit as EtkezesFeltetel).Szamlalo));
                    command.ExecuteNonQuery();
                }
                else if (modosit is EtrendFeltetel)
                {
                    command.Parameters.Clear();
                    command.CommandText =
                        "UPDATE [EtrendFeltetel] SET [Datum1] = @v2, [Etkezes1]=@v3, [Datum2] = @v4, [Etkezes2]=@v5, [Koltsegmin]=@v6, [Orommax]=@v7, [Maxpenz]=@v8, [PenznemMegnevezes]=@v9, [Solverelrejt]=@v10, [Folytonosmodell]=@v11,[Numvaltozatossag]=@v12,[Maxfutasiido]=@v13,[NaptarbaMent]=@v14 WHERE [FelhasznaloNevHash] = @v1;";
                    command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", Convert.ToDateTime((modosit as EtrendFeltetel).Datum1));
                    command.Parameters.AddWithValue("@v3", (byte)(modosit as EtrendFeltetel).Etkezes1);
                    command.Parameters.AddWithValue("@v4", Convert.ToDateTime((modosit as EtrendFeltetel).Datum2));
                    command.Parameters.AddWithValue("@v5", (byte)(modosit as EtrendFeltetel).Etkezes2);
                    command.Parameters.AddWithValue("@v6", Convert.ToByte((modosit as EtrendFeltetel).Koltsegmin));
                    command.Parameters.AddWithValue("@v7", Convert.ToByte((modosit as EtrendFeltetel).Orommax));
                    command.Parameters.AddWithValue("@v8", (float)(modosit as EtrendFeltetel).Maxpenz);
                    command.Parameters.AddWithValue("@v9", (modosit as EtrendFeltetel).Penz.Megnevezes);
                    command.Parameters.AddWithValue("@v10", Convert.ToByte((modosit as EtrendFeltetel).Solverelrejt));
                    command.Parameters.AddWithValue("@v11", Convert.ToByte((modosit as EtrendFeltetel).Folytonosmodell));
                    command.Parameters.AddWithValue("@v12", Convert.ToInt32((modosit as EtrendFeltetel).Numvaltozatossag));
                    command.Parameters.AddWithValue("@v13", Convert.ToInt32((modosit as EtrendFeltetel).Maxfutasiido));
                    command.Parameters.AddWithValue("@v14", Convert.ToByte((modosit as EtrendFeltetel).NaptarbaMent));
                    command.ExecuteNonQuery();
                }
                else if (modosit is EtrendIdopont)
                {
                    command.Parameters.Clear();
                    command.CommandText =
                        "UPDATE [EtrendIdopont] SET [ReggeliIdopont] = @v2, [EbedIdopont]=@v3, [VacsoraIdopont] = @v4 WHERE [FelhasznaloNevHash] = @v1;";
                    command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", Convert.ToDateTime((modosit as EtrendIdopont).Datum[0]));
                    command.Parameters.AddWithValue("@v3", Convert.ToDateTime((modosit as EtrendIdopont).Datum[1]));
                    command.Parameters.AddWithValue("@v4", Convert.ToDateTime((modosit as EtrendIdopont).Datum[2]));
                    command.ExecuteNonQuery();
                }
                else if (modosit is Elelmiszer)
                {
                    if (modosit is Etel)
                    {
                        command.Parameters.Clear();
                        command.CommandText =
                            "UPDATE [ElelmiszerTomeg] SET [MertekegysegMegnevezes]=@v3, [Tomeg]=@v4, [MinTomeg]=@v5, [MaxTomeg]=@v6, [MaxTomegE]=@v7   WHERE [FelhasznaloNevHash]=@v1 AND [ElelmiszerMegnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (modosit as Etel).Megnevezes);
                        command.Parameters.AddWithValue("@v3", (modosit as Etel).TomegMertek.Megnevezes);
                        command.Parameters.AddWithValue("@v4", (float)((modosit as Etel).EgysegTomegMennyiseg));
                        command.Parameters.AddWithValue("@v5", (float)((modosit as Etel).MinTomeg));
                        command.Parameters.AddWithValue("@v6", (float)((modosit as Etel).MaxTomeg));
                        command.Parameters.AddWithValue("@v7", Convert.ToByte((modosit as Etel).MaxTomegE));
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "UPDATE [Etel] SET [ElelmiszerTipus] = @v3, [Orom] = @v4, [EgysegTobbszorose] = @v5, [ReggelinelFogyaszthato]=@v6,[EbednelFogyaszthato]=@v7,[VacsoranalFogyaszthato]=@v8,[Valtozatossag]=@v9,[Hasznalhato]=@v10,[TomegMertekegysegMegnevezes]=@v11 WHERE [FelhasznaloNevHash]=@v1 AND [Megnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (modosit as Etel).Megnevezes);
                        command.Parameters.AddWithValue("@v3", Convert.ToByte(0));//étel=0
                        command.Parameters.AddWithValue("@v4", Convert.ToByte((modosit as Etel).Orom));
                        command.Parameters.AddWithValue("@v5", Convert.ToByte((modosit as Etel).EgysegTobbszorose));
                        command.Parameters.AddWithValue("@v6", Convert.ToByte((modosit as Etel).Fogyaszthato[0]));
                        command.Parameters.AddWithValue("@v7", Convert.ToByte((modosit as Etel).Fogyaszthato[1]));
                        command.Parameters.AddWithValue("@v8", Convert.ToByte((modosit as Etel).Fogyaszthato[2]));
                        command.Parameters.AddWithValue("@v9", Convert.ToByte((modosit as Etel).Valtozatossag));
                        command.Parameters.AddWithValue("@v10", Convert.ToByte((modosit as Etel).Hasznalhato));
                        command.Parameters.AddWithValue("@v11", (modosit as Etel).TomegMertek.Megnevezes);
                        command.ExecuteNonQuery();
                    }
                    else if (modosit is Ital)
                    {
                        command.Parameters.Clear();
                        command.CommandText =
                            "UPDATE [ElelmiszerUrtartalom] SET [MertekegysegMegnevezes]=@v3, [Urtartalom]=@v4, [MinUrtartalom]=@v5, [MaxUrtartalom]=@v6, [MaxUrtartalomE]=@v7   WHERE [FelhasznaloNevHash]=@v1 AND [ElelmiszerMegnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (modosit as Ital).Megnevezes);
                        command.Parameters.AddWithValue("@v3", (modosit as Ital).Urmertek.Megnevezes);
                        command.Parameters.AddWithValue("@v4", (float)((modosit as Ital).EgysegUrTartalomMennyiseg));
                        command.Parameters.AddWithValue("@v5", (float)((modosit as Ital).MinUrTartalom));
                        command.Parameters.AddWithValue("@v6", (float)((modosit as Ital).MaxUrTartalom));
                        command.Parameters.AddWithValue("@v7", Convert.ToByte((modosit as Ital).MaxUrTartalomE));
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "UPDATE [Ital] SET [ElelmiszerTipus] = @v3, [Orom] = @v4, [EgysegTobbszorose] = @v5, [ReggelinelFogyaszthato]=@v6,[EbednelFogyaszthato]=@v7,[VacsoranalFogyaszthato]=@v8,[Valtozatossag]=@v9,[Hasznalhato]=@v10,[UrMertekegysegMegnevezes]=@v11 WHERE [FelhasznaloNevHash]=@v1 AND [Megnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (modosit as Ital).Megnevezes);
                        command.Parameters.AddWithValue("@v3", Convert.ToByte(1));//ital=1
                        command.Parameters.AddWithValue("@v4", Convert.ToByte((modosit as Ital).Orom));
                        command.Parameters.AddWithValue("@v5", Convert.ToByte((modosit as Ital).EgysegTobbszorose));
                        command.Parameters.AddWithValue("@v6", Convert.ToByte((modosit as Ital).Fogyaszthato[0]));
                        command.Parameters.AddWithValue("@v7", Convert.ToByte((modosit as Ital).Fogyaszthato[1]));
                        command.Parameters.AddWithValue("@v8", Convert.ToByte((modosit as Ital).Fogyaszthato[2]));
                        command.Parameters.AddWithValue("@v9", Convert.ToByte((modosit as Ital).Valtozatossag));
                        command.Parameters.AddWithValue("@v10", Convert.ToByte((modosit as Ital).Hasznalhato));
                        command.Parameters.AddWithValue("@v11", (modosit as Ital).Urmertek.Megnevezes);
                        command.ExecuteNonQuery();
                    }
                    else if (modosit is Menu)
                    {
                        command.Parameters.Clear();
                        command.CommandText =
                            "UPDATE [ElelmiszerTomeg] SET [MertekegysegMegnevezes]=@v3, [Tomeg]=@v4  WHERE [FelhasznaloNevHash]=@v1 AND [ElelmiszerMegnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (modosit as Menu).Megnevezes);
                        command.Parameters.AddWithValue("@v3", (modosit as Menu).TomegMertek.Megnevezes);
                        command.Parameters.AddWithValue("@v4", (float)((modosit as Menu).EgysegTomegMennyiseg));
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "UPDATE [ElelmiszerUrtartalom] SET [MertekegysegMegnevezes]=@v3, [Urtartalom]=@v4 WHERE [FelhasznaloNevHash]=@v1 AND [ElelmiszerMegnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (modosit as Menu).Megnevezes);
                        command.Parameters.AddWithValue("@v3", (modosit as Menu).Urmertek.Megnevezes);
                        command.Parameters.AddWithValue("@v4", (float)((modosit as Menu).EgysegUrTartalomMennyiseg));
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "DELETE FROM [ElelmiszerElelmiszer] WHERE [FelhasznaloNevHash]=@v1 AND [Megnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (modosit as Menu).Megnevezes);
                        command.ExecuteNonQuery();

                        foreach (KeyValuePair<Elelmiszer, double> item in (modosit as Menu).Osszetevo)
                        {
                            command.Parameters.Clear();
                            command.CommandText =
                                "INSERT INTO [ElelmiszerElelmiszer] VALUES(@v1,@v2,@v3,@v4)";
                            command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                            command.Parameters.AddWithValue("@v2", (modosit as Menu).Megnevezes);
                            command.Parameters.AddWithValue("@v3", item.Key.Megnevezes);
                            command.Parameters.AddWithValue("@v4", Convert.ToSingle(item.Value));
                            command.ExecuteNonQuery();
                        }


                        command.Parameters.Clear();
                        command.CommandText =
                            "UPDATE [Menu] SET [ElelmiszerTipus] = @v3, [Orom] = @v4, [EgysegTobbszorose] = @v5, [ReggelinelFogyaszthato]=@v6,[EbednelFogyaszthato]=@v7,[VacsoranalFogyaszthato]=@v8,[Valtozatossag]=@v9,[Hasznalhato]=@v10,[TomegMertekegysegMegnevezes]=@v11,[UrMertekegysegMegnevezes]=@v12,[MaxDarab]=@v13 WHERE [FelhasznaloNevHash]=@v1 AND [Megnevezes]=@v2;";
                        command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (modosit as Menu).Megnevezes);
                        command.Parameters.AddWithValue("@v3", Convert.ToByte(2));//menü=2
                        command.Parameters.AddWithValue("@v4", Convert.ToByte((modosit as Menu).Orom));
                        command.Parameters.AddWithValue("@v5", Convert.ToByte((modosit as Menu).EgysegTobbszorose));
                        command.Parameters.AddWithValue("@v6", Convert.ToByte((modosit as Menu).Fogyaszthato[0]));
                        command.Parameters.AddWithValue("@v7", Convert.ToByte((modosit as Menu).Fogyaszthato[1]));
                        command.Parameters.AddWithValue("@v8", Convert.ToByte((modosit as Menu).Fogyaszthato[2]));
                        command.Parameters.AddWithValue("@v9", Convert.ToByte((modosit as Menu).Valtozatossag));
                        command.Parameters.AddWithValue("@v10", Convert.ToByte((modosit as Menu).Hasznalhato));
                        command.Parameters.AddWithValue("@v11", (modosit as Menu).Urmertek.Megnevezes);
                        command.Parameters.AddWithValue("@v12", (modosit as Menu).TomegMertek.Megnevezes);
                        command.Parameters.AddWithValue("@v13", Convert.ToByte((modosit as Menu).MaxDarab));
                        command.ExecuteNonQuery();
                    }

                    command.Parameters.Clear();
                    command.CommandText =
                        "UPDATE [ElelmiszerPenznem] SET [PenznemMegnevezes]=@v3,[Ar]=@v4 WHERE [FelhasznaloNevHash]=@v1 AND [ElelmiszerMegnevezes]=@v2;";
                    command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (modosit as Elelmiszer).Megnevezes);
                    command.Parameters.AddWithValue("@v3", (modosit as Elelmiszer).Penz.Megnevezes);
                    command.Parameters.AddWithValue("@v4", Convert.ToSingle((modosit as Elelmiszer).Ar));
                    command.ExecuteNonQuery();

                    foreach (KeyValuePair<Tapanyag, double> item in (modosit as Elelmiszer).TapanyagTartalom)
                    {
                        command.Parameters.Clear();
                        command.CommandText =
                            "UPDATE [ElelmiszerTapanyag] SET [Mennyiseg]=@v4 WHERE [FelhasznaloNevHash]=@v1 AND [ElelmiszerMegnevezes]=@v2 AND [TapanyagMegnevezes]=@v3;";
                        command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                        command.Parameters.AddWithValue("@v2", (modosit as Elelmiszer).Megnevezes);
                        command.Parameters.AddWithValue("@v3", item.Key.Megnevezes);
                        command.Parameters.AddWithValue("@v4", Convert.ToSingle(item.Value));
                        command.ExecuteNonQuery();
                    }

                    command.Parameters.Clear();
                    command.CommandText =
                        "UPDATE [Elelmiszer] SET [Orom] = @v4, [EgysegTobbszorose] = @v5, [ReggelinelFogyaszthato]=@v6,[EbednelFogyaszthato]=@v7,[VacsoranalFogyaszthato]=@v8,[Valtozatossag]=@v9,[Hasznalhato]=@v10  WHERE [FelhasznaloNevHash]=@v1 AND [Megnevezes]=@v2;";
                    command.Parameters.AddWithValue("@v1", modosit.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (modosit as Elelmiszer).Megnevezes);
                    command.Parameters.AddWithValue("@v4", Convert.ToByte((modosit as Elelmiszer).Orom));
                    command.Parameters.AddWithValue("@v5", Convert.ToByte((modosit as Elelmiszer).EgysegTobbszorose));
                    command.Parameters.AddWithValue("@v6", Convert.ToByte((modosit as Elelmiszer).Fogyaszthato[0]));
                    command.Parameters.AddWithValue("@v7", Convert.ToByte((modosit as Elelmiszer).Fogyaszthato[1]));
                    command.Parameters.AddWithValue("@v8", Convert.ToByte((modosit as Elelmiszer).Fogyaszthato[2]));
                    command.Parameters.AddWithValue("@v9", Convert.ToByte((modosit as Elelmiszer).Valtozatossag));
                    command.Parameters.AddWithValue("@v10", Convert.ToByte((modosit as Elelmiszer).Hasznalhato));
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new ABKivetel("The modification is unsuccessful!", e);
            }
        }

        public static void TorolEtrendElelmiszer()
        {
            SqlTransaction transaction = null;
            try
            {
                command.Parameters.Clear();
                transaction = connection.BeginTransaction("Torol");
                command.Transaction = transaction;
                command.Parameters.AddWithValue("@v1", ABKezelo.GetCurrentUser());

                command.CommandText = "DELETE FROM [EtrendElelmiszer] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new ABKivetel("The deletion is unsuccessful!", e);
            }
        }

        public static void Torol(EtrendAdat torol)
        {
            SqlTransaction transaction = null;
            try
            {
                command.Parameters.Clear();
                transaction = connection.BeginTransaction("Torol");
                command.Transaction = transaction;
                command.Parameters.AddWithValue("@v1", torol.FelhasznaloNevHash);
                string st = null;

                if (torol is Felhasznalo) st = "Felhasznalo";
                else if (torol is Mertekegyseg) st = "Mertekegyseg";
                else if (torol is Penznem) st = "Penznem";
                else if (torol is Tapanyag) st = "Tapanyag";
                else if (torol is Elelmiszer) st = "Elelmiszer";


                if (torol is Felhasznalo)
                {
                    command.CommandText = "DELETE FROM [EtrendAdat] WHERE [FelhasznaloNevHash] = @v1;";
                    command.ExecuteNonQuery();
                }
                else if (torol is Penznem)
                {
                    command.CommandText = $"DELETE FROM [{st}] WHERE [FelhasznaloNevHash] = @v1 AND [Megnevezes]=@v2;";
                    command.Parameters.AddWithValue("@v2", (torol as Penznem).Megnevezes);
                    command.ExecuteNonQuery();
                }
                else if (torol is Mertekegyseg)
                {
                    command.CommandText = $"DELETE FROM [{st}] WHERE [FelhasznaloNevHash] = @v1 AND [Megnevezes]=@v2;";
                    command.Parameters.AddWithValue("@v2", (torol as Mertekegyseg).Megnevezes);
                    command.ExecuteNonQuery();
                }
                else if (torol is Tapanyag)
                {
                    // a tápanyag mértékegységét is töröljük
                    command.Parameters.Clear();
                    command.CommandText = $"DELETE FROM [TapanyagMertekegyseg] WHERE [FelhasznaloNevHash] = @v1 AND [TapanyagMegnevezes]=@v2;";
                    command.Parameters.AddWithValue("@v1", torol.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (torol as Tapanyag).Megnevezes);
                    command.ExecuteNonQuery();

                    command.CommandText =
                            $"DELETE FROM [ElelmiszerTapanyag] WHERE [FelhasznaloNevHash] = @v1 AND [TapanyagMegnevezes]=@v2;";
                    command.ExecuteNonQuery();

                    command.CommandText = $"DELETE FROM [{st}] WHERE [FelhasznaloNevHash] = @v1 AND [Megnevezes]=@v2;";
                    command.ExecuteNonQuery();
                }
                else if (torol is EtkezesFeltetel)
                {
                    command.Parameters.Clear();
                    command.CommandText = $"DELETE FROM [EtkezesFeltetel] WHERE [FelhasznaloNevHash] = @v1 AND [Eltipus2]=@v2 AND [Ettipus2]=@v3 AND [Szamlalo]=@v4;";
                    command.Parameters.AddWithValue("@v1", torol.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (torol as EtkezesFeltetel).Eltipus2);
                    command.Parameters.AddWithValue("@v3", (torol as EtkezesFeltetel).Ettipus2);
                    command.Parameters.AddWithValue("@v4", (torol as EtkezesFeltetel).Szamlalo);
                    command.ExecuteNonQuery();
                }
                else if (torol is EtrendFeltetel)
                {
                    command.Parameters.Clear();
                    command.CommandText = $"DELETE FROM [EtrendFeltetel] WHERE [FelhasznaloNevHash] = @v1;";
                    command.Parameters.AddWithValue("@v1", torol.FelhasznaloNevHash);
                    command.ExecuteNonQuery();
                }
                else if (torol is EtrendElelmiszer)
                {
                    command.Parameters.Clear();
                    command.CommandText = $"DELETE FROM [EtrendElelmiszer] WHERE [FelhasznaloNevHash] = @v1 AND [Datum]=@v2 AND [Etkezes]=@v3 AND [ElelmiszerMegnevezes]=@v4;";
                    command.Parameters.AddWithValue("@v1", torol.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (torol as EtrendElelmiszer).Datum);
                    command.Parameters.AddWithValue("@v3", Convert.ToByte((torol as EtrendElelmiszer).Etkezes));
                    command.Parameters.AddWithValue("@v4", (torol as EtrendElelmiszer).Elelmiszer.Megnevezes);
                    command.ExecuteNonQuery();
                }
                else if (torol is EtrendIdopont)
                {
                    command.Parameters.Clear();
                    command.CommandText = $"DELETE FROM [EtrendIdopont] WHERE [FelhasznaloNevHash] = @v1;";
                    command.Parameters.AddWithValue("@v1", torol.FelhasznaloNevHash);
                    command.ExecuteNonQuery();
                }
                else if (torol is Elelmiszer)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@v1", torol.FelhasznaloNevHash);
                    command.Parameters.AddWithValue("@v2", (torol as Elelmiszer).Megnevezes);

                    //étrend élelmiszerből a törlése
                    command.CommandText =
                        $"DELETE FROM [EtrendElelmiszer] WHERE [FelhasznaloNevHash] = @v1 AND [ElelmiszerMegnevezes]=@v2;";
                    command.ExecuteNonQuery();

                    if (torol is Etel)
                    {
                        command.CommandText =
                            $"DELETE FROM [ElelmiszerTomeg] WHERE [FelhasznaloNevHash] = @v1 AND [ElelmiszerMegnevezes]=@v2;";
                        command.ExecuteNonQuery();

                        command.CommandText = $"DELETE FROM [Etel] WHERE [FelhasznaloNevHash] = @v1 AND [Megnevezes]=@v2;";
                        command.ExecuteNonQuery();

                    }
                    else if (torol is Ital)
                    {
                        command.CommandText =
                            $"DELETE FROM [ElelmiszerUrtartalom] WHERE [FelhasznaloNevHash] = @v1 AND [ElelmiszerMegnevezes]=@v2;";
                        command.ExecuteNonQuery();

                        command.CommandText = $"DELETE FROM [Ital] WHERE [FelhasznaloNevHash] = @v1 AND [Megnevezes]=@v2;";
                        command.ExecuteNonQuery();
                    }
                    else if (torol is Menu)
                    {

                        command.CommandText =
                            $"DELETE FROM [ElelmiszerTomeg] WHERE [FelhasznaloNevHash] = @v1 AND [ElelmiszerMegnevezes]=@v2;";
                        command.ExecuteNonQuery();

                        command.CommandText =
                            $"DELETE FROM [ElelmiszerUrtartalom] WHERE [FelhasznaloNevHash] = @v1 AND [ElelmiszerMegnevezes]=@v2;";
                        command.ExecuteNonQuery();

                        command.CommandText = $"DELETE FROM [Menu] WHERE [FelhasznaloNevHash] = @v1 AND [Megnevezes]=@v2;";
                        command.ExecuteNonQuery();
                    }

                    command.CommandText =
                        $"DELETE FROM [ElelmiszerTapanyag] WHERE [FelhasznaloNevHash] = @v1 AND [ElelmiszerMegnevezes]=@v2;";
                    command.ExecuteNonQuery();

                    command.CommandText =
                        $"DELETE FROM [ElelmiszerElelmiszer] WHERE [FelhasznaloNevHash] = @v1 AND ([Megnevezes]=@v2 OR [Osszetevo]=@v2);";
                    command.ExecuteNonQuery();

                    command.CommandText =
                        $"DELETE FROM [ElelmiszerPenznem] WHERE [FelhasznaloNevHash] = @v1 AND [ElelmiszerMegnevezes]=@v2;";
                    command.ExecuteNonQuery();

                    command.CommandText =
                        $"DELETE FROM [Elelmiszer] WHERE [FelhasznaloNevHash] = @v1 AND [Megnevezes]=@v2;";
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new ABKivetel("The deletion is unsuccessful!", e);
            }
        }

        public static void TorolMinden()
        {
            SqlTransaction transaction = null;
            try
            {
                command.Parameters.Clear();
                transaction = connection.BeginTransaction("Torol");
                command.Transaction = transaction;
                command.Parameters.AddWithValue("@v1", ABKezelo.GetCurrentUser());

                command.CommandText =
                    $"DELETE FROM [ElelmiszerTapanyag] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                command.CommandText =
                    $"DELETE FROM [ElelmiszerElelmiszer] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                command.CommandText =
                    $"DELETE FROM [ElelmiszerPenznem] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                command.CommandText =
                    $"DELETE FROM [ElelmiszerTomeg] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                command.CommandText =
                    $"DELETE FROM [ElelmiszerUrtartalom] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                command.CommandText =
                    $"DELETE FROM [Menu] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                command.CommandText =
                    $"DELETE FROM [Etel] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                command.CommandText =
                    $"DELETE FROM [Ital] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                //étrendefeltételek törlése
                command.CommandText =
                    $"DELETE FROM [EtrendFeltetel] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                //étrend élelmiszerek törlése
                command.CommandText =
                    $"DELETE FROM [EtrendElelmiszer] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                //étkezésfeltételek törlése
                command.CommandText =
                    $"DELETE FROM [EtkezesFeltetel] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                //étrendidőpontok törlése
                command.CommandText =
                    $"DELETE FROM [EtrendIdopont] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                //élelmiszerek törlése
                command.CommandText =
                    $"DELETE FROM [Elelmiszer] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                //tápanyag,pénznem, mértékegységek törlése
                command.CommandText =
                    $"DELETE FROM [TapanyagMertekegyseg] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                command.CommandText =
                    $"DELETE FROM [Tapanyag] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                command.CommandText =
                    $"DELETE FROM [Penznem] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                command.CommandText =
                    $"DELETE FROM [Mertekegyseg] WHERE [FelhasznaloNevHash] = @v1;";
                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new ABKivetel("The deletion is unsuccessful!", e);
            }
        }

        public static List<ABHiba> Csoportosbeszuras(List<EtrendAdat> etrendadatok)
        {
            List<ABHiba> hibak = new List<ABHiba>();
            foreach (EtrendAdat item in etrendadatok)
            {
                try
                {
                    Beszuras(item);
                }
                catch (Exception)
                {
                    hibak.Add(new ABHiba("Unsuccessful group insert!", item));
                }
            }
            return hibak;
        }
    }
}
