using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Loto
{
    class ProgowanieGradientowe
    {
        public static unsafe bool* ProgójGradientowo(byte* Wejście,Size Rozmiar,bool Odwrotność=false)
        {
            int X = Rozmiar.Width;
            int Y = Rozmiar.Height;
            int Suma = 0, SumaWag = 0;
            int XP = X + 1;
            byte * PoczątekDziałania = Wejście + XP;
            int IlośćPentli = Rozmiar.Width * Rozmiar.Height;
            IlośćPentli -= XP * 2;
            for (int i = 0; i < IlośćPentli; i++,PoczątekDziałania++)
            {
                int RóżnicaX =  PoczątekDziałania[-1] - PoczątekDziałania[1], RóźnicaY = PoczątekDziałania[-X] - PoczątekDziałania[X];
                RóżnicaX = RóżnicaX < 0 ? -RóżnicaX : RóżnicaX;
                RóźnicaY = RóźnicaY < 0 ? -RóźnicaY : RóźnicaY;
                int Waga = RóźnicaY < RóżnicaX ? RóżnicaX : RóźnicaY;
                Suma += Waga **PoczątekDziałania;
                SumaWag += Waga;
            }
            Suma /= SumaWag;
            return Otsu.Progój(Odwrotność, X * Y, Suma, Wejście);
        }
    }
}
