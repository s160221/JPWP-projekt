﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

namespace Play_with_English
{
    public partial class Test : Form
    {
        public static string kategoria { get; set; }

        // slownik informujacy, czy napisy zostaly rozmieszczone w docelowych miejscach
        protected Dictionary<Label, bool> wykorzystany = new Dictionary<Label, bool>();

        protected int liczbaKlikniec = 0;   // licznik klinkniec dla metody otwierania Menu
        FlowLayoutPanel flp = new FlowLayoutPanel();    // utworzenie panelu dla Menu

        protected ushort wynik;     // zmienna przechowujaca wynik uzyskany w tescie
        protected ushort etap;      // zmienna okreslajaca etap testu
        


        Dictionary<string, string> dict = new Dictionary<string, string>();   // slownik do przechowywania obrazow i ich podpisow
        string[] img = new string[10];              // tablica do przechowywania 10 wylosowanych obrazow
        string[] podpis = new string[10];         // tablica do przechowywania 10 nazw odpowiadajacych wylosowanym obrazom

        public Test()
        {
            InitializeComponent();

            Form1.reOpen = false;       // powrot do stanu pierwotnego - wymazanie statusu ponownego otwarcia okna
            etap = 1;                   // rozpoczecie od pierwszego etapu

            if (kategoria == "TestGlowny")  // jezeli wybrano test glowny
            {
                List<string> listaZdjec = new List<string>();   // lista na zdjecia dla testu glownego
                string nazwa;   // kontener na wartosci slownika

                foreach (kategorie kat in Enum.GetValues(typeof(kategorie)))    // iterowanie po wszystkich kategoriach
                {
                    if (kat.ToString() != "TestGlowny")
                    {
                        string fullPath = Path.GetFullPath(kat.ToString());      // wyszukanie folderu zawierajacego elementy z zadanej kategorii
                        listaZdjec.AddRange(Directory.GetFiles(fullPath));       // dodanie danej kategorii do listy
                    }
                }

                foreach (var naz in listaZdjec)
                {
                    //Bitmap bmp = null;

                    try
                    {
                        //bmp = new Bitmap(naz);    // utworzenie bitmapy na podstawie sciezki
                        nazwa = naz.Split('.')[0];
                        nazwa = nazwa.Split('\\').Last();   // zebranie nazwy pliku jako klucza
                        nazwa = nazwa.Split('_')[0];
                        nazwa = char.ToUpper(nazwa[0]) + nazwa.Substring(1);    // zamiana pierwszej litery na wielka
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        continue;
                    }

                    dict.Add(naz, nazwa);
                }
            }
            else                             // jezeli wybrano inny test
            {
                string fullPath = Path.GetFullPath(kategoria);      // wyszukanie folderu zawierajacego elementy z zadanej dla testu kategorii

                string[] zdjecia = Directory.GetFiles(fullPath);    // zapisanie sciezek do poszczegolnych obrazow
                string nazwa;   // kontener na wartosci slownika

                foreach (var naz in zdjecia)
                {
                    //Bitmap bmp = null;

                    try
                    {
                        //bmp = new Bitmap(naz);    // utworzenie bitmapy na podstawie sciezki
                        nazwa = naz.Split('.')[0];
                        nazwa = nazwa.Split('\\').Last();   // zebranie nazwy pliku jako klucza
                        nazwa = nazwa.Split('_')[0];
                        nazwa = char.ToUpper(nazwa[0]) + nazwa.Substring(1);    // zamiana pierwszej litery na wielka
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        continue;
                    }

                    dict.Add(naz, nazwa);
                }
            }
            
            Random losuj = new Random();
            dict = dict.OrderBy(x => losuj.Next()).Take(10).ToDictionary(elem => elem.Key, elem => elem.Value);    // przetasowanie elementow w slowniku
            //dict = dict.Take(10).ToDictionary(elem => elem.Key, elem => elem.Value);                             // wziecie 10 elementow (warunek dla testu glownego)

            int i = 0;      // licznik dla petli par w slowniku

            foreach (KeyValuePair<string, string> obraz in dict)
            {
                img[i] = obraz.Key;
                podpis[i] = obraz.Value;
                i++;
            }

            Etap();
        }

        // Metoda odpowiedzialna za realizację odpowiedniego etapu testu
        private void Etap()
        {
            if (etap == 1 || etap == 3)
            {
                Czyszczenie();
                Pytanie1();
            }
            if (etap == 2 || etap == 4)
            {
                Czyszczenie();
                Pytanie2();
            }
            if (etap == 5)
            {
                EndTestPart();                  // zakonczenie etapu testu
            }
        }

        // Metoda odpowiedzialna za czyszczenie ekranu przed rozpoczeciem kolejnego pytania w tescie
        private void Czyszczenie()
        {
            wykorzystany.Clear();             // wyczyszczenie slownika wykorzystanych elementow

            List<Control> doUsuniecia = new List<Control>();    // utworzenie listy dla elementow przeznaczonych do usuniecia

            foreach (Control c in splitContainer1.Panel1.Controls)
            {
                if (c.Name != "labWynik" && c.Name != "Menu")
                {
                    doUsuniecia.Add(c);     // dodanie do listy wszystkich elementow utworzonych dynamicznie
                }
            }

            foreach (Control c in doUsuniecia)
            {
                splitContainer1.Panel1.Controls.Remove(c);      // usuniecie elementow utworzonych dynamicznie
                c.Dispose();
            }
        }

        // Metoda odpowiedzialna za realizację pytania 1.
        private void Pytanie1()
        {
            Random rand = new Random();
            string[] image = new string[4];               // tablica do przechowania obrazow
            //string[] oPrztasowane = new string[4];        // tablica na przetasowane obrazy
            string[] odpowiedzi = new string[4];        // tablica na podpisy obrazkow
            //string[] pPrzetasowane = new string[4];     // tablica na przetasowane podpisy

            if (etap == 1)
            {
                image = img.Take(4).ToArray();         // wybranie pierwszych czterech obrazow
                odpowiedzi = podpis.Take(4).ToArray();    // wybranie pierwszych czterech etykiet
            }
            else
            {
                image = img.Skip(5).Take(4).ToArray();         // wybranie od 6 do 9 obrazu
                odpowiedzi = podpis.Skip(5).Take(4).ToArray();    // wybranie od 6 do 9 etykiety
            }

            image = image.OrderBy(r => rand.Next()).ToArray();           // przemieszanie wybranych obrazow
            odpowiedzi = odpowiedzi.OrderBy(r => rand.Next()).ToArray();     // przemieszanie wybranych etykiet

            Panel[] pan = new Panel[4];
            PictureBox[] pb = new PictureBox[4];
            Label[] lab = new Label[4];
            int x = 0;  // zmienna uzywana przy ustawianiu polozenia w osi x
            int y = 0;  // zmienna uzywana przy ustawianiu polozenia w osi y

            for (int i = 0; i < 4; i++)
            {
                pan[i] = new Panel
                {
                    Size = new Size(250, 50),
                    BorderStyle = BorderStyle.Fixed3D,
                    Name = "Panel" + (i + 2),
                    AllowDrop = true    // obszar docelowy przeciagania
                };
                pan[i].DragEnter += new DragEventHandler(pan_DragEnter);
                pan[i].DragDrop += new DragEventHandler(pan1_DragDrop);

                pb[i] = new PictureBox
                {
                    Name = "PictureBox" + (i + 1),
                    Size = new Size(250, 250),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    ImageLocation = image[i]
                };

                if (i % 2 == 0)           // rozmieszczenie parzystych paneli
                {
                    pan[i].Location = new Point(180, 380 + y);
                    pb[i].Location = new Point(180, 110 + y);
                    x += 640;
                }
                else                    // rozmieszczenie nieparzystych paneli
                {
                    pan[i].Location = new Point(180 + x, 380 + y);
                    pb[i].Location = new Point(180 + x, 110 + y);
                    x = 0;
                    y += 360;
                }

                splitContainer1.Panel1.Controls.Add(pan[i]);
                splitContainer1.Panel1.Controls.Add(pb[i]);

                pan[i].Tag = dict[pb[i].ImageLocation];     // powiazanie panelu z odpowiadajacym mu zdjeciem

                lab[i] = new Label
                {
                    Size = new Size(250, 50),
                    Font = new Font("Arial", 20),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = System.Drawing.Color.White,
                    Name = "Label" + (i + 1),
                    Text = odpowiedzi[i],
                    Location = new Point(50 + 300 * i, 55)
                };
                lab[i].MouseDown += new MouseEventHandler(lab1_MouseDown);   // uruchomienie przeciagania po wcisnieciu myszki
                splitContainer1.Panel2.Controls.Add(lab[i]);

                wykorzystany.Add(lab[i], false);     // dodanie informacji o tym, ze dana etykieta nie zostala jeszcze rozmieszczona
            }
        }

        // Metoda odpowiedzialna za realizację pytania 2.
        private void Pytanie2()
        {
            Random rand = new Random();
            string image = null;     // obiekt do przechowania obrazu z tablicy
            string nazwa;          // obiekt do przechowania nazwy obrazu

            if (etap == 2)
            {
                image = img[4];        // wybranie piatego obrazu z tablicy
                nazwa = podpis[4];    // wybranie piatej nazwy obrazu z tablicy
            }
            else
            {
                image = img[9];         // wybranie ostatniego obrazu z tablicy
                nazwa = podpis[9];      // wybranie ostatniej nazwy obrazu z tablicy
            }

            char[] odpowiedz = nazwa.ToArray();     // tablica znakow podpisu obrazka
            char[] przetasowane = odpowiedz.OrderBy(r => rand.Next()).ToArray();    // tablica na przetasowane znaki podpsiu obrazka

            PictureBox pb = new PictureBox
            {
                Name = "PictureBox1",
                Size = new Size(460, 460),
                Location = new Point(402, 150),
                SizeMode = PictureBoxSizeMode.StretchImage,
                ImageLocation = image
            };
            splitContainer1.Panel1.Controls.Add(pb);

            Panel[] pan = new Panel[przetasowane.Length];
            Label[] lab = new Label[przetasowane.Length];

            for (int i = 0; i < przetasowane.Length; i++)
            {
                pan[i] = new Panel
                {
                    Size = new Size(60, 60),
                    BorderStyle = BorderStyle.Fixed3D,
                    Name = "Panel" + (i + 2),
                    Tag = odpowiedz[i].ToString(),  // przypisanie odpowiedniej litery do danego panelu
                    AllowDrop = true    // obszar docelowy przeciagania
                };
                pan[i].DragEnter += new DragEventHandler(pan_DragEnter);
                pan[i].DragDrop += new DragEventHandler(pan2_DragDrop);

                lab[i] = new Label
                {
                    Size = new Size(60, 60),
                    Font = new Font("Arial", 20),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White,
                    Name = "Label" + (i + 1),
                    Text = przetasowane[i].ToString(),   // przypisanie losowej litery z podpisu obrazka do etykiety
                    Location = new Point(50 + 300 * i, 50)
                };
                lab[i].MouseDown += new MouseEventHandler(lab2_MouseDown);   // uruchomienie przeciagania po wcisnieciu myszki

                if (przetasowane.Length % 2 == 0)           // rozmieszczenie dla parzystej liczby paneli i etykiet
                {
                    pan[i].Location = new Point(632 + ((i - (przetasowane.Length / 2)) * 80) + 10, 630);
                    lab[i].Location = new Point(632 + ((i - (przetasowane.Length / 2)) * 80) + 10, 50);
                }
                else                                        // rozmieszczenie dla nieparzystej liczby paneli i etykie
                {
                    pan[i].Location = new Point(632 + ((i - ((przetasowane.Length - 1) / 2)) * 80) - 30, 630);
                    lab[i].Location = new Point(632 + ((i - ((przetasowane.Length - 1) / 2)) * 80) - 30, 50);
                }

                splitContainer1.Panel1.Controls.Add(pan[i]);
                splitContainer1.Panel2.Controls.Add(lab[i]);

                bool wartosc;   // zmienna zwracana w przypadku, gdy dana etykieta zostal juz wprowadzona do slownika

                if (wykorzystany.TryGetValue(lab[i], out wartosc))
                {
                    wykorzystany[lab[i]] = false;      // nadpisanie wartosci w przypadku, gdy dany klucz istnieje
                }
                else
                {
                    wykorzystany.Add(lab[i], false);   // dodanie pary klucz - wartosc w przypadku, gdy nie byla jeszcze wpisana
                }
            }
        }

        // Metoda przemieszczania niewykorzystanych etykiet dla pytania 1.
        private void lab1_MouseDown(object sender, MouseEventArgs e)
        {
            Label lab = (Label)sender;     // obsluga zdarzenia dla etykiety

            if (wykorzystany[lab] == false)
            {
                lab.DoDragDrop(lab, DragDropEffects.Move);  // ustawienie przemieszczania niewykorzystanego elementu dla realizacji Drag&Drop
            }
            else
            {
                lab.DoDragDrop(lab, DragDropEffects.None);  // wykorzystany element nie moze byc przemieszczany
            }
        }

        // Metoda przemieszczania niewykorzystanych etykiet dla pytania 2.
        private void lab2_MouseDown(object sender, MouseEventArgs e)
        {
            Label lab = (Label)sender;     // obsluga zdarzenia dla etykiety
            lab.DoDragDrop(lab, DragDropEffects.Move);  // ustawienie przemieszczania niewykorzystanego elementu dla realizacji Drag&Drop
        }

        // Metoda wywolana podczas przeciagania elementu nad docelowym obszarem dla pytania 1.
        private void pan_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Label)))
            {
                e.Effect = DragDropEffects.Move;   // ustawienie efektu dla przeciagania etykiety w dozwolone miejsce
            }

        }

        // Metoda wywolana po upuszczeniu przeciaganego elementu dla pytania 1.
        private void pan1_DragDrop(object sender, DragEventArgs e)
        {
            var lab = (Label)e.Data.GetData(typeof(Label));   // pobranie danych przeciaganej etykiety
            var pan = (Panel)sender;

            if (lab.Text != (string)pan.Tag)
            {
                e.Effect = DragDropEffects.None;
                MessageBox.Show("Błędna odpowiedź!");

                if (wynik > 0)
                {
                    wynik--;    // odjecie punktu w przypadku, gdy wynik jest wyzszy od zera
                    labWynik.Text = "WYNIK: " + wynik;
                }
            }
            else
            {
                lab.Parent = pan;     // ustawienie obszaru docelowego jako nowego rodzica przeciagnietej etykiety
                lab.Location = new Point((pan.Width - lab.Width) / 2, (pan.Height - lab.Height) / 2); // wysrodkowanie etykiety w obszarze docelowym

                MessageBox.Show("Brawo! Poprawna odpowiedź!");

                wynik++;
                labWynik.Text = "WYNIK: " + wynik;
                pan.AllowDrop = false;
                wykorzystany[lab] = true;         // zawarcie informacji o tym, ze etykieta zostala wykorzystana

                ushort rozmieszczone = 0;         // zmienna sprawdzajaca ilosc rozmieszczonych elementow w tescie

                foreach (KeyValuePair<Label, bool> wyk in wykorzystany)
                {
                    if (wyk.Value)
                    {
                        rozmieszczone++;
                    }
                }

                if ((etap == 1 || etap == 3) && rozmieszczone == 4)
                {
                    etap++;     // przejscie do nastepnego etapu
                    Etap();     // wywolanie metody w momencie rozmieszczenia wszystkich etykiet dla etapu 1. lub 3.
                }
            }
        }

        // Metoda wywolana po upuszczeniu przeciaganego elementu dla pytania 2.
        private void pan2_DragDrop(object sender, DragEventArgs e)
        {
            var lab = (Label)e.Data.GetData(typeof(Label));   // pobranie danych przeciaganej etykiety
            var pan = (Panel)sender;

            lab.Parent = pan;     // ustawienie obszaru docelowego jako nowego rodzica przeciagnietej etykiety
            lab.Location = new Point((pan.Width - lab.Width) / 2, (pan.Height - lab.Height) / 2); // wysrodkowanie etykiety w obszarze docelowym

            string utworzonaNazwa = "";     // kontener na nazwe utworzona z rozmieszczonych liter

            // ustawianie wszystkich paneli z literami jako zablokowanych dla innych liter
            foreach (Control c in splitContainer1.Panel1.Controls.OfType<Panel>())
            {
                if (c.HasChildren)
                {
                    c.AllowDrop = false;    // zablokowanie przeciagania
                    Label l = (Label)c.GetChildAtPoint(new Point(0,0));     // pobranie informacji o etykiecie znajdujacej sie w panelu
                    utworzonaNazwa += l.Text;       // tworzenie slowa z kolejno rozmieszczonych liter
                }
                else
                {
                    c.AllowDrop = true;
                }
            }

            wykorzystany[lab] = true;   // oznaczenie, ze dana litera zostala wykorzystana

            bool rozmieszczone = true;      // flaga rozmieszczenia wszystkich liter

            foreach (KeyValuePair<Label, bool> rozm in wykorzystany)
            {
                if (!rozm.Value)
                {
                    rozmieszczone = false;
                }
            }

            // sprawdzenie, czy wszystkie etykiety zostaly rozmieszczone
            if (rozmieszczone)
            {
                // jezeli etykiety tworza dobra sekwencje
                if ((etap == 2 && utworzonaNazwa == dict[img[4]]) || (etap == 4 && utworzonaNazwa == dict[img[9]]))
                {
                    MessageBox.Show("Brawo! Poprawna odpowiedź!");

                    wynik++;    // dodanie punktu
                    labWynik.Text = "WYNIK: " + wynik;

                    etap++;
                    Etap();     // przejscie do kolejnego etapu
                }
                else            // jezeli etykiety tworza zla sekwencje
                {
                    MessageBox.Show("Błędna odpowiedź!");

                    if (wynik > 0)
                    {
                        wynik--;    // odjecie punktu w przypadku, gdy wynik jest wyzszy od zera
                        labWynik.Text = "WYNIK: " + wynik;
                    }

                    Etap();     // powtorzenie etapu
                }

            }
        }

        // Metoda realizowana po odpowiedzi na wszystkie pytania
        private void EndTestPart()
        {
            ZapisywanieWyniku();

            // utworzenie okna
            Form informacja = new Form();
            informacja.Size = new Size(600, 350);
            informacja.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            informacja.MaximizeBox = false;
            informacja.MinimizeBox = false;
            informacja.Text = "Play with English";
            informacja.FormBorderStyle = FormBorderStyle.FixedDialog;
            informacja.ControlBox = false;      // usuniecie przycisku zamykania okna

            Label tekst = new Label();
            tekst.Text = "Ukończono test! Twój wynik to: " + wynik + " punktów (" + (wynik*10) + "%). "
                + "Swoje wyniki zawsze możesz sprawdzić wybierając odpowiednią opcję z Menu. Możesz teraz "
                + "przejść do ekranu głównego lub powtórzyć test. Test możesz uruchomić również z poziomu "
                + "ekranu głównego.";
            tekst.Font = new Font("Arial", 16);
            tekst.TextAlign = ContentAlignment.MiddleCenter;
            tekst.Location = new Point(10, 25);
            tekst.Size = new Size(550, 100);

            Button koniec = new Button();
            koniec.Text = "Przejdź do ekranu głównego";
            koniec.Location = new Point(115, 200);
            koniec.Size = new Size(150, 75);
            koniec.Click += new EventHandler(koniec_Click);

            Button powtorz = new Button();
            powtorz.Text = "Powtórz test";
            powtorz.Location = new Point(325, 200);
            powtorz.Size = new Size(150, 75);
            powtorz.Click += new EventHandler(powtorz_Click);

            informacja.Controls.Add(tekst);
            informacja.Controls.Add(koniec);
            informacja.Controls.Add(powtorz);
            informacja.ShowDialog();
        }

        // Metoda realizowana po odpowiedzi na wszystkie pytania
        private void ZapisywanieWyniku()
        {
            Wyniki wyniki = null;   // do przechowywania obiektow klasy Wynik

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//Wyniki.xml"; // sciezka do pliku z danymi
            XmlSerializer serializer = new XmlSerializer(typeof(Wyniki));   // obiekt odpowiadajacy serializacje i deserializacje danych
            StreamReader reader = new StreamReader(path);   // wczytanie z pliku o zadanej sciezce

            wyniki = (Wyniki)serializer.Deserialize(reader);    // pobranie danych z drzewa xml do listy wynikow
            reader.Close();

            foreach (Wynik w in wyniki.listaWynikow)
            {
                if (w.kategoria.ToString() == kategoria)    // wyszukanie elementu z zadanej kategorii
                {
                    if (w.wynikTestu < wynik)
                    {
                        w.wynikTestu = wynik;       // zaktualizowanie wyniku, jezeli uzyskano wyzszy
                    }

                    w.odblokowane = true;
                    break;
                }
            }

            FileStream fs = File.Create(path);  // nadpisanie pliku o zadanej sciezce
            serializer.Serialize(fs, wyniki);   // utworzenie drzewa xml ze zaktualizowanymi danymi obiektow klasy Wynik
            fs.Close();
        }

        // Metoda realizowana po wyborze przycisku powrotu do ekranu glownego
        private void koniec_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;    // pobranie informacji o przycisku
            Form inf = (Form)btn.Parent;    // pobranie informacji o formie bedacej rodzicem przycisku

            inf.Close();
            inf.Dispose();
            this.Close();
            this.Dispose();
        }

        // Metoda realizowana po wyborze przycisku powtorzenia etapu testu
        private void powtorz_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;    // pobranie informacji o przycisku
            Form inf = (Form)btn.Parent;    // pobranie informacji o formie bedacej rodzicem przycisku

            var testForm = new Test();
            inf.Hide();                     // ukrycie dialogboxa
            this.Hide();                    // ukrycie formy etapu testu
            testForm.ShowDialog();
            inf.Close();
            inf.Dispose();
            this.Close();
            this.Dispose();
        }

        // Metoda odpowiedzialna za utworzenie menu i wprowadzenie przyciskow funkcyjnych
        private void Menu_Click(object sender, EventArgs e)
        {
            liczbaKlikniec++;

            flp.Size = new Size(Menu.Width - 5, 170);
            flp.Location = new Point(1263 - Menu.Width, Menu.Height + 2);
            flp.BorderStyle = BorderStyle.FixedSingle;

            Button wyniki = new Button
            {
                Text = "Wyniki",
                Location = new Point(0, 0),
                Size = new Size(Menu.Width - 15, 30)
            };
            wyniki.Click += new EventHandler(wyniki_Click);

            Button pomoc = new Button
            {
                Text = "Pomoc",
                Location = new Point(0, wyniki.Height),
                Size = new Size(Menu.Width - 15, 30)
            };
            pomoc.Click += new EventHandler(pomoc_Click);

            Button powrot = new Button
            {
                Text = "Powrót do okna startowego",
                Location = new Point(0, wyniki.Height * 2),
                Size = new Size(Menu.Width - 15, 50)
            };
            powrot.Click += new EventHandler(powrot_Click);

            Button wyjscie = new Button
            {
                Text = "Wyjście",
                Location = new Point(0, wyniki.Height * 3 + 20),
                Size = new Size(Menu.Width - 15, 30)
            };
            wyjscie.Click += new EventHandler(wyjscie_Click);

            flp.Controls.Add(wyniki);
            flp.Controls.Add(pomoc);
            flp.Controls.Add(powrot);
            flp.Controls.Add(wyjscie);
            this.Controls.Add(flp);

            if (liczbaKlikniec % 2 != 0)      // w przpyadku nieparzystego klikniecia ikony - menu jest wyswietlane
            {
                flp.Visible = true;
                flp.BringToFront();
            }
            else
            {
                flp.Visible = false;        // w przypadku parzystego klikniecia ikony - menu jest ukrywane
            }
        }

        // Metoda realizowana po wyborze przycisku wyswietlenia wynikow
        private void wyniki_Click(object sender, EventArgs e)
        {
            Plik.Odczyt();
        }

        // Metoda realizowana po wyborze przycisku wyswietlenia pomocy
        private void pomoc_Click(object sender, EventArgs e)
        {
            Form pom = new Form
            {
                Size = new Size(600, 400),
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                Text = "Play with English",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ControlBox = false      // usuniecie przycisku zamykania okna
            };

            Label tekst = new Label
            {
                Text = "Test składa się z 4 pytań - po 2 pytania do 2 typów odpowiedzi. Pierwszy typ odpowiedzi opiera się na "
                + "przeciąganiu angielskich nazw odpowiadających przedstawionym obrazkom. Natomiast drugi typ odpowiedzi opiera się "
                + "przeciąganiu bloczków z pojedynczymi literami, tak aby utworzyły poprawne słowo odpowiadające przedstawionemu "
                + "obrazkowi. Za poprawną odpowiedź otrzymuje się +1, a za błędną -1 punkt (przy czym wynik nie może być niższy niż 0). "
                + "Maksymalna liczba punktów do uzyskania w teście wynosi 10 punktów. Należy pamiętać, że opuszczenie testu przed jego "
                + "ukończeniem skutkuje wynikiem 0. Powodzenia!",
                Font = new Font("Arial", 14),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 10),
                Size = new Size(550, 265)
            };

            Button ok = new Button
            {
                Text = "Ok",
                Location = new Point(240, 300),
                Size = new Size(100, 30)
            };
            ok.Click += new EventHandler(ok_Click);

            pom.Controls.Add(tekst);
            pom.Controls.Add(ok);
            pom.ShowDialog();
        }

        // Metoda realizowana po wyborze przycisku Ok w oknie dialogowym pomocy
        private void ok_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;    // pobranie informacji o przycisku
            Form par = (Form)btn.Parent;    // pobranie informacji o formie bedacej rodzicem przycisku (oknie pomocy)

            par.Close();    // zamkniecie okna pomocy
            par.Dispose();
        }

        // Metoda realizowana po wyborze przycisku powrotu
        private void powrot_Click(object sender, EventArgs e)
        {
            Form uwaga = new Form
            {
                Size = new Size(600, 350),
                StartPosition = System.Windows.Forms.FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                Text = "Play with English",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ControlBox = false      // usuniecie przycisku zamykania okna
            };

            Label tekst = new Label
            {
                Text = "Uwaga! Jeżeli zdecydujesz się na wyjście przed ukończeniem testu, Twój wynik w tym podejściu wyniesie 0. "
                + "Czy jesteś pewien, że chcesz zakończyć teraz test?",
                Font = new Font("Arial", 14),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 10),
                Size = new Size(550, 100)
            };

            Button tak = new Button
            {
                Text = "Tak",
                Location = new Point(115, 200),
                Size = new Size(150, 75)
            };
            tak.Click += new EventHandler(tak_Click);

            Button anuluj = new Button
            {
                Text = "Anuluj",
                Location = new Point(325, 200),
                Size = new Size(150, 75)
            };
            anuluj.Click += new EventHandler(ok_Click);

            uwaga.Controls.Add(tekst);
            uwaga.Controls.Add(tak);
            uwaga.Controls.Add(anuluj);
            uwaga.ShowDialog();
        }

        // Metoda realizowana po wyborze przycisku Tak w oknie powrotu
        private void tak_Click(object sender, EventArgs e)
        {
            wynik = 0;

            Button btn = (Button)sender;    // pobranie informacji o przycisku
            Form par = (Form)btn.Parent;    // pobranie informacji o formie bedacej rodzicem przycisku (oknie ostrzezenia)

            par.Close();    // zamkniecie okna ostrzezenia
            par.Dispose();
            this.Close();   // zamkniecie okna etapu nauki
            this.Dispose();
        }

        // Metoda realizowana po wyborze przycisku wyjscie
        private void wyjscie_Click(object sender, EventArgs e)
        {
            Form uwaga = new Form
            {
                Size = new Size(600, 350),
                StartPosition = System.Windows.Forms.FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                Text = "Play with English",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ControlBox = false      // usuniecie przycisku zamykania okna
            };

            Label tekst = new Label
            {
                Text = "Uwaga! Jeżeli zdecydujesz się na wyjście przed ukończeniem testu, Twój wynik w tym podejściu wyniesie 0. "
                + "Czy jesteś pewien, że chcesz zakończyć teraz test?",
                Font = new Font("Arial", 14),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 10),
                Size = new Size(550, 100)
            };

            Button tak = new Button
            {
                Text = "Tak",
                Location = new Point(115, 200),
                Size = new Size(150, 75)
            };
            tak.Click += new EventHandler(tak1_Click);

            Button anuluj = new Button
            {
                Text = "Anuluj",
                Location = new Point(325, 200),
                Size = new Size(150, 75)
            };
            anuluj.Click += new EventHandler(ok_Click);

            uwaga.Controls.Add(tekst);
            uwaga.Controls.Add(tak);
            uwaga.Controls.Add(anuluj);
            uwaga.ShowDialog();
        }

        // Metoda realizowana po wyborze przycisku Tak w oknie wyjscia
        private void tak1_Click(object sender, EventArgs e)
        {
            wynik = 0;

            Button btn = (Button)sender;    // pobranie informacji o przycisku
            Form par = (Form)btn.Parent;    // pobranie informacji o formie bedacej rodzicem przycisku (oknie ostrzezenia)

            par.Close();    // zamkniecie okna ostrzezenia
            par.Dispose();
            System.Environment.Exit(0);     // zamkniecie aplikacji
        }
    }
}
