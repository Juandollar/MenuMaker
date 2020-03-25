using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace EtrendKeszito
{
    static class Naptar
    {
        static string[] Scopes = { CalendarService.Scope.Calendar };

        static string ApplicationName = "Google Calendar API .NET Quickstart";

        public static void NaptarbaRak()
        {
            try
            {
                UserCredential credential;

                using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                Event evnt = new Event();

                EtrendFeltetel e = (EtrendFeltetel)ABKezelo.Kiolvasas().Where(x => x is EtrendFeltetel).First();

                var l = ABKezelo.Kiolvasas().Where(x => x is EtrendElelmiszer).ToList();

                DateTime e1 = e.Datum1;
                DateTime e2 = e.Datum2;
                e1 = new DateTime(e1.Year, e1.Month, e1.Day, 0, 0, 0);
                e2 = new DateTime(e2.Year, e2.Month, e2.Day, 0, 0, 0);
                DateTime d = e1;

                int[] ido = new int[3];

                var v = ABKezelo.Kiolvasas().Where(x => x is EtrendIdopont).ToList();
                if (v.Count > 0)
                {
                    EtrendIdopont et = (EtrendIdopont)v.First();
                    for (int i = 0; i < 3; i++) ido[i] = 60 * et.Datum[i].Hour + et.Datum[i].Minute;
                }
                else
                {
                    for (int i = 0; i < 3; i++) ido[i] = 60 * (7 + 6 * i) + 30;
                }

                int num = 0;
                while (d <= e2)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (d == e1 && i < (int)e.Etkezes1) continue;
                        if (d == e2 && i > (int)e.Etkezes2) continue;

                        var l2 = l.Where(x =>
                            (x as EtrendElelmiszer).Datum == d && (int)(x as EtrendElelmiszer).Etkezes == i).ToList();

                        string str = ((EtkezesTipus)i).ToString() + ":";
                        foreach (EtrendElelmiszer item in l2)
                        {
                            Elelmiszer elelmiszer = item.Elelmiszer;
                            if (elelmiszer is Etel)
                                str += String.Format("{0:0.0} {1} {2};", item.Tomeg, item.Tomegmertekegyseg.Megnevezes,
                                    elelmiszer.Megnevezes);
                            else if (elelmiszer is Ital)
                                str += String.Format("{0:0.0} {1} {2};", item.Urmertek, item.Urmertekegyseg.Megnevezes,
                                    elelmiszer.Megnevezes);
                            else
                                str += String.Format("{0:0.0} {1} [{2:0.0} {3}, {4:0.0} {5}];", item.Val,
                                    elelmiszer.Megnevezes, item.Tomeg,
                                    item.Tomegmertekegyseg.Megnevezes, item.Urmertek, item.Urmertekegyseg.Megnevezes);
                        }

                        evnt.Summary = str;
                        evnt.Description = ((EtkezesTipus)i).ToString();

                        DateTime d1 = d + TimeSpan.FromMinutes(ido[i]);
                        DateTime d2 = d1 + TimeSpan.FromMinutes(30); //30 perc hosszú étkezés

                        // időpontok beállitása
                        evnt.Start = new EventDateTime() { DateTime = d1 };
                        evnt.End = new EventDateTime() { DateTime = d2 };

                        String calendarId = "primary";
                        service.Events.Insert(evnt, calendarId).Execute();
                    }

                    d = d.AddDays(1);
                }
                Logolas.Ment("Successful save to calendar");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
