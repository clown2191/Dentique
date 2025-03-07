using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace Dentique
{
    internal class CircularPictureBox: PictureBox
    {
        public CircularPictureBox()
        {
            this.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Habilitar anti-aliasing para bordes suaves
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Crear un círculo y recortar la imagen dentro
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(0, 0, this.Width - 1, this.Height - 1);
                pe.Graphics.SetClip(path);

                // Dibujar la imagen con mejor calidad
                if (this.Image != null)
                {
                    pe.Graphics.DrawImage(this.Image, new Rectangle(0, 0, this.Width, this.Height));
                }

                // Restaurar la región de recorte
                this.Region = new Region(path);
            }
        }
    }
}
