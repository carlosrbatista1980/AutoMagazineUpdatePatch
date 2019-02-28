using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace AutoMagazineUpdatePatch
{
    public class GifImage
    {
        private Image gifImage;
        private FrameDimension dimension;
        private int frameCount;
        private int currentFrame = -1;
        private bool reverse;
        private int step = 1;

        public GifImage(string path)
        {
            try
            {
                gifImage = Image.FromFile(path);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                MessageBox.Show("Não encontrado " + ex.Message, "Gerenciador do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            //initialize
            dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
            //pega o GUID
            //frames totais do gif
            frameCount = gifImage.GetFrameCount(dimension);
        }

        public bool ReverseAtEnd
        {
            //quando chegar ao fim volta ao primeiro frame
            get { return reverse; }
            set { reverse = value; }
        }

        public Image GetNextFrame()
        {

            currentFrame += step;

            //se o gif chegar ao limit
            if (currentFrame >= frameCount || currentFrame < 1)
            {
                if (reverse)
                {
                    step *= -1;
                    //...reverter contage
                    //aplicar
                    currentFrame += step;
                }
                else
                {
                    currentFrame = 0;
                    //...começa no zero
                }
            }
            return GetFrame(currentFrame);
        }

        public Image GetFrame(int index)
        {
            gifImage.SelectActiveFrame(dimension, index);
            //melhorar  esta parte
            //encontra o gif
            return (Image)gifImage.Clone();
            //melhorar esta parte
            //devolve a copia
        }
    }
}
