using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using GrafyShp.Icer;
using Loto.SiecNeuronowa;
using System.IO;
namespace Loto  
{
    public unsafe static class PodziałLinik
    {
        const int WielkośćTebów = 40;
        static float MinimalnePodobieństwo = 0.6f;
        const float MaksymalneOdalenie = 0.1f;
        const int MinimalnyKontrast = 14;
        public static IntPtr ObrazMiejsc;
        public static SiecNeuronowa.SieciRywalizujące<string> Sieć;
        static PodziałLinik()
        {
            Sieć = SiecNeuronowa.SieciRywalizujące<string>.Wczytaj(StałeGlobalne.NazwaPlikuRywalizującejSieci, Czytnik);
        }

        public static string Czytnik(BinaryReader br)
        {
            string s = br.ReadString();
            if (s.Length==2)
            {
                if (s[1] == 'D')
                    return Convert.ToString(s[0]).ToUpper();
                else
                    return "%";
            }
            return s;
        }

        public static Linika[] PodzielNaLiniki(ref bool* c, byte* monohromatyczny, Bitmap SamLoto,out ZdjecieZPozycją Logo, bool ProgowanieRegionalne = false)
        {
            Logo = null;
            int Długość = SamLoto.Width * SamLoto.Height;
            bool* ObrazKopis = (bool*)Marshal.AllocHGlobal(Długość);
            OperacjeNaStrumieniu.Kopiuj(ObrazKopis, c, Długość);
            GC.Collect();
            List<ZdjecieZPozycją> ListaZdjęć = new List<ZdjecieZPozycją>();
            ObszaryNaZdzieciaZPozycją(ref c, monohromatyczny, SamLoto, ProgowanieRegionalne, ObrazKopis, ListaZdjęć,out Logo);
            Marshal.FreeHGlobal((IntPtr)ObrazKopis);
            //ObróćPoKodzie(SamLoto, ListaZdjęć);
            int PrógMimalnejOdległośc = Convert.ToInt32(MaksymalneOdalenie * SamLoto.Width);
            //w tym momencie lotek powinien być w odpowiedniej pozycji
            ListaZdjęć.Sort(new DoKwadratów.SortowanieWzgledemX());
            List<Linika> ZwracanePrzedPrzemianą = PrzydzielanieDoLinik(ListaZdjęć, PrógMimalnejOdległośc);
            ScalajLiniki(ZwracanePrzedPrzemianą);
            ZlepianieLinijek zl = new ZlepianieLinijek(Sieć);
            zl.ZlepPeknieteLitery(ZwracanePrzedPrzemianą, c, SamLoto.Width);
            ListaZdjęć = Rozczep(ZwracanePrzedPrzemianą);
            ZwracanePrzedPrzemianą = PrzydzielanieDoLinik(ListaZdjęć, PrógMimalnejOdległośc);
            PosortujY(ZwracanePrzedPrzemianą);
            ScalajLiniki(ZwracanePrzedPrzemianą);
            return ZwracanePrzedPrzemianą.ToArray();

            //return StaryPodziałNaLiniki(SamLoto, ListaZdjęć, sklaerY, TBY);
        }
        private static void ScalajLiniki(List<Linika> zwracanePrzedPrzemianą)
        {
            foreach (var item in zwracanePrzedPrzemianą)
            {
                item.ObliczŚrednie();
            }
            for (int i = 0; i < zwracanePrzedPrzemianą.Count; i++)
            {
                for (int j = i + 1; j < zwracanePrzedPrzemianą.Count; j++)
                {
                    if (zwracanePrzedPrzemianą[i].SprawdźPodobieństwo(zwracanePrzedPrzemianą[j]))
                    {
                        zwracanePrzedPrzemianą[i].ScalLinike(zwracanePrzedPrzemianą[j]);
                        zwracanePrzedPrzemianą[i].ObliczŚrednie();
                        zwracanePrzedPrzemianą.RemoveAt(j);
                        j--;
                    }
                }
            }
        }
        private static void PosortujY(List<Linika> zwracanePrzedPrzemianą)
        {
            zwracanePrzedPrzemianą.Sort(new Comparison<Linika>((Linika a, Linika b) => { return a.Min - b.Min; }));
        }

        private static List<ZdjecieZPozycją> Rozczep(List<Linika> zwracanePrzedPrzemianą)
        {
            List<ZdjecieZPozycją> zw = new List<ZdjecieZPozycją>();
            foreach (var item in zwracanePrzedPrzemianą)
            {
                foreach (var item2 in item.ListaZZdjeciami)
                {
                    zw.Add(item2);
                }
            }
            return zw;
        }
        private static List<Linika> PrzydzielanieDoLinik(List<ZdjecieZPozycją> ListaZdjęć, int PrógMimalnejOdległośc)
        {
            List<Linika> ZwracanePrzedPrzemianą = new List<Linika>();
            foreach (var item in ListaZdjęć)
            {
                int Index = -1;
                float NajbardziejPodobny = float.MinValue;
                for (int i = 0; i < ZwracanePrzedPrzemianą.Count; i++)
                {
                    ZdjecieZPozycją Ostanij = ZwracanePrzedPrzemianą[i].Last();
                    float Podobieństwo = Matematyka.PodobieństwoYProstokontów(Ostanij.Obszar, item.Obszar);
                    if (Podobieństwo > NajbardziejPodobny && item.Obszar.X - Ostanij.Obszar.X < PrógMimalnejOdległośc)
                    {
                        Index = i;
                        NajbardziejPodobny = Podobieństwo;
                    }
                }
                if (NajbardziejPodobny > MinimalnePodobieństwo)
                {
                    ZwracanePrzedPrzemianą[Index].Add(item);
                }
                else
                {
                    ZwracanePrzedPrzemianą.Add(new Linika());
                    ZwracanePrzedPrzemianą.Last().Add(item);
                }
            }

            return ZwracanePrzedPrzemianą;
        }

      


        private static void ObszaryNaZdzieciaZPozycją(ref bool* c, byte* monohromatyczny, Bitmap SamLoto, bool ProgowanieRegionalne, bool* ObrazKopis, List<ZdjecieZPozycją> ListaZdjęć,out ZdjecieZPozycją Logo)
        {
            Logo = null;
            int* MapaSpójnychObszarów;
            float NajbardzjeLogowaty = 0;
            List<WstepnePrzygotowanie.ObiektNaMapie> Obszary = WstepnePrzygotowanie.ZnajdźOpszary(ref c, SamLoto.Width, SamLoto.Height, out MapaSpójnychObszarów);
            foreach (var item in Obszary)
            {

                    Rectangle rl = new Rectangle(item.MinX, item.MinY, item.MaxX - item.MinX + 1, item.MaxY - item.MinY + 1);
                    if (ProgowanieRegionalne)
                    {
                        byte* obszar = (byte*)OperacjeNaStrumieniu.KopiujFragment(monohromatyczny, new Size(SamLoto.Width, SamLoto.Height), rl);
                        int[] hist = Otsu.PobierzHistogram(rl.Size, obszar);
                        Marshal.FreeHGlobal((IntPtr)obszar);
                        //if (BadanieHistogramu.Kontrast(hist) < MinimalnyKontrast)
                        //{
                        //    continue;
                        //}
                    }
                    ZdjecieZPozycją z = new ZdjecieZPozycją();
                    z.Obszar = rl;
                    z.Moc = item.Moc;
                    z.ObiektNaMapie = item;
                    z.ObliczPodobieństwo(c, SamLoto.Width,Sieć);

                    //usóń w dalszej fazie i Usń też Kopie
                    z.ObrazWBool = WstepnePrzygotowanie.PobierzObszar(ObrazKopis, z, SamLoto.Width, SamLoto.Height);

                if (SprawdźPoprawne(item))
                {
                    ListaZdjęć.Add(z);

                }
                
                float Logowatość = ObliczLogowatość(z);
                if (Logowatość>NajbardzjeLogowaty)
                {
                    NajbardzjeLogowaty = Logowatość;
                    Logo = z;
                }
            }
            Marshal.FreeHGlobal((IntPtr) MapaSpójnychObszarów);
        }
        const int MaksymalnaWielkośćLoga = 100000;
        const float WypełnienieLoga = 0.6f;
        private static float ObliczLogowatość(ZdjecieZPozycją z)
        {
            float X= z.Obszar.Width, Y = z.Obszar.Height;
            float zw = X * Y;
            if (zw > MaksymalnaWielkośćLoga)
                return 0;
            float Min, Max;
            if (X<Y)
            {
                Min = X;
                Max = Y;
            }
            else
            {
                Min = Y;
                Max = X;
            }
            zw *= Min / Max;

            zw *= Matematyka.Podobność(WypełnienieLoga, z.Wypełninienie());
            return zw;
        }

        const float MinWypełnineie = 0.04f;
        const float MaksymalneWYpełninie = 1.9f;//wartość od 0 do 1
        const int MinmalnaMoc = 3;
        private static bool SprawdźPoprawne(WstepnePrzygotowanie.ObiektNaMapie obj)
        {
            float Wielkość = (obj.MaxX - obj.MinX) * (obj.MaxY - obj.MinY);
            float Wypełnienie = (obj.Moc / Wielkość);
            return Wypełnienie > MinWypełnineie && obj.Moc > MinimalnePodobieństwo&& Wielkość < 2000&& MaksymalneWYpełninie>Wypełnienie;
        }

        /*
      private static void ObróćPoKodzie(Bitmap SamLoto, List<ZdjecieZPozycją> ListaZdjęć)
      {
          float skalerX = (float)(WielkośćTebów - 1) / (SamLoto.Width - 1);
          float sklaerY = (float)(WielkośćTebów - 1) / (SamLoto.Height - 1);
          int[] TBX = new int[WielkośćTebów];
          int[] TBY = new int[WielkośćTebów];
          WyznaczTBy(ListaZdjęć, skalerX, sklaerY, TBX, TBY);

          int IlośćZerX = 0, IlośćZerY = 0;
          IlośćZerX = TBX.Zlicz(0);
          IlośćZerY = TBY.Zlicz(0);
          if (IlośćZerY < IlośćZerX)
          {
              Obróto90(SamLoto, ListaZdjęć, ref TBX, ref TBY);
              ZerujTBy(out TBX, out TBY);
              WyznaczTBy(ListaZdjęć, skalerX, sklaerY, TBX, TBY);
          }
      }
       private static List<ZdjecieZPozycją>[] StaryPodziałNaLiniki(Bitmap SamLoto, List<ZdjecieZPozycją> ListaZdjęć, float sklaerY, int[] TBY)
      {
          int NumerLiniki = 0;
          bool StanZeroCzyNieZero = true;

          for (int i = 0; i < TBY.Length; i++)
          {
              if (StanZeroCzyNieZero)
              {
                  if (TBY[i] != 0)
                  {

                      StanZeroCzyNieZero = !StanZeroCzyNieZero;
                  }
              }
              else
              {
                  if (TBY[i] == 0)
                  {
                      StanZeroCzyNieZero = !StanZeroCzyNieZero;
                      NumerLiniki++;
                  }
              }
              TBY[i] = NumerLiniki;
          }
          List<ZdjecieZPozycją>[] lz = new List<ZdjecieZPozycją>[NumerLiniki + 1];
          for (int i = 0; i < lz.Length; i++)
          {
              lz[i] = new List<ZdjecieZPozycją>();
          }
          foreach (var item in ListaZdjęć)
          {
              Point S = item.Sierodek();
              lz[TBY[S.Y.SkalujWInt(sklaerY)]].Add(item);
              item.Zdjecie = SamLoto.Clone(item.Obszar, PixelFormat.Format24bppRgb);

          }
          return lz;
      }

      private static void ZerujTBy(out int[] TBX, out int[] TBY)
      {
          TBX = new int[WielkośćTebów];
          TBY = new int[WielkośćTebów];
      }

      private static void Obróto90(Bitmap SamLoto, List<ZdjecieZPozycją> ListaZdjęć, ref int[] TBX, ref int[] TBY)
      {
          SamLoto.RotateFlip(RotateFlipType.Rotate90FlipY);
          Podstawowe.Zamiana(ref TBX, ref TBY);
          foreach (var item in ListaZdjęć)
          {
              item.Obszar = item.Obszar.Translacja(new Size(SamLoto.Width, SamLoto.Height));
          }
      }

      private static void WyznaczTBy(List<ZdjecieZPozycją> ListaZdjęć, float skalerX, float sklaerY, int[] TBX, int[] TBY)
      {
          foreach (var item in ListaZdjęć)
          {
              Point Pozycja = item.Sierodek();
              TBX[Pozycja.X.SkalujWInt(skalerX)]++;
              TBY[Pozycja.Y.SkalujWInt(sklaerY)]++;
          }
      }
      */
  

    }
}
