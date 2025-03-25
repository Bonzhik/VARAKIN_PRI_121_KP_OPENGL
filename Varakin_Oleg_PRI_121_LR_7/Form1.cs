using System.Drawing.Imaging;
using Tao.DevIl;
using Tao.FreeGlut;
using Tao.OpenGl;

namespace Varakin_Oleg_PRI_121_LR_7
{
    public partial class Form1 : Form
    {
        int angle = 5, angleX = -90, angleY = 0, angleZ = 90;
        double translateX = 250, translateY = -40, translateZ = -50;
        int alpha = 0;
        int beta = 1;
        bool isGrayscale = false;
        uint mGlTextureObject;
        uint mGlTextureObject2;
        uint[] spriteTextures = new uint[3];
        int spriteFrameIndex = 0;
        int currentTextureIndex = 0;
        bool textureIsLoad;

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);
            Gl.glClearColor(255, 255, 255, 1);
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(60, (float)AnT.Width / (float)AnT.Height, 0.1, 900);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            loadImage();
            Bitmap fractalBitmap = GenerateMandelbrotFractal(500, 500, -2.5, 1.5, -2, 2);
            ApplyTexture(fractalBitmap);
            RenderTimer.Start();
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            currentTextureIndex = (currentTextureIndex + 1) % spriteTextures.Length;
            Draw();
        }

        private void AnT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                translateX = translateX + Math.Sin(((90 - angleZ) * Math.PI) / 180);
                translateY = translateY - Math.Cos(((90 - angleZ) * Math.PI) / 180);
            }
            if (e.KeyCode == Keys.D)
            {
                translateX = translateX - Math.Sin(((90 - angleZ) * Math.PI) / 180);
                translateY = translateY + Math.Cos(((90 - angleZ) * Math.PI) / 180);
            }
            if (e.KeyCode == Keys.W)
            {
                translateX = translateX - 2 * Math.Cos(((angleZ - 90) * Math.PI) / 180);
                translateY = translateY + 2 * Math.Sin(((angleZ - 90) * Math.PI) / 180);
            }
            if (e.KeyCode == Keys.S)
            {
                translateX = translateX + Math.Cos(((angleZ - 90) * Math.PI) / 180);
                translateY = translateY - Math.Sin(((angleZ - 90) * Math.PI) / 180);
            }

            if (e.KeyCode == Keys.Q)
            {
                angleZ -= angle;
            }
            if (e.KeyCode == Keys.E)
            {
                angleZ += angle;
            }
        }

        private void Draw()
        {
            float lightIntensity = (float)Math.Abs(Math.Sin(Math.PI * beta / 360));

            if (textureIsLoad)
            {
                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

                Gl.glClearColor(0.66f * lightIntensity, 0.98f * lightIntensity, 0.99f * lightIntensity, 1);
                Gl.glLoadIdentity();
                Gl.glPushMatrix();
                Gl.glRotated(angleX, 1, 0, 0); Gl.glRotated(angleY, 0, 1, 0); Gl.glRotated(angleZ, 0, 0, 1);
                Gl.glTranslated(translateX, translateY, translateZ);

                //Земля
                Gl.glColor3f(0.1f * lightIntensity, 0.1f * lightIntensity, 0.1f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(1000, 500, 0);
                Gl.glVertex3d(-1000, 500, 0);
                Gl.glVertex3d(-1000, -500, 0);
                Gl.glVertex3d(1000, -500, 0);
                Gl.glEnd();

                // Включаем текстуры
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, spriteTextures[currentTextureIndex]);

                // Отрисовка вертикальной плоскости с текстурой
                Gl.glColor3f(1.0f, 1.0f, 1.0f);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3d(199, 150, 25);
                Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3d(199, 40, 25);
                Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3d(199, 40, 75);
                Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3d(199, 150, 75);
                Gl.glEnd();

                Gl.glDisable(Gl.GL_TEXTURE_2D);

                // Пол (серый)
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject2);

                Gl.glColor3f(0.5f, 0.5f, 0.5f);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex3d(200, -10, 5);
                Gl.glTexCoord2f(1.0f, 0.0f); Gl.glVertex3d(-100, -10, 5);
                Gl.glTexCoord2f(1.0f, 1.0f); Gl.glVertex3d(-100, 200, 5);
                Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex3d(200, 200, 5);
                Gl.glEnd();

                Gl.glDisable(Gl.GL_TEXTURE_2D);

                // Потолок (серый)
                Gl.glColor3f(0.5f * lightIntensity, 0.5f * lightIntensity, 0.5f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(200, -10, 100);
                Gl.glVertex3d(-100, -10, 100);
                Gl.glVertex3d(-100, 200, 100);
                Gl.glVertex3d(200, 200, 100);
                Gl.glEnd();

                //Стена основная
                Gl.glPushMatrix();
                Gl.glColor3f(0.8f * lightIntensity, 0.8f * lightIntensity, 0.8f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(200, 200, 0);
                Gl.glVertex3d(200, -10, 0);
                Gl.glVertex3d(200, -10, 100);
                Gl.glVertex3d(200, 200, 100);
                Gl.glEnd();

                // стена правая c окном
                Gl.glPushMatrix();
                Gl.glColor3f(0.8f * lightIntensity, 0.8f * lightIntensity, 0.8f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(-100, -10, 0);
                Gl.glVertex3d(-25, -10, 0);
                Gl.glVertex3d(-25, -10, 100);
                Gl.glVertex3d(-100, -10, 100);
                Gl.glEnd();

                Gl.glPushMatrix();
                Gl.glColor3f(0.8f * lightIntensity, 0.8f * lightIntensity, 0.8f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(125, -10, 0);
                Gl.glVertex3d(200, -10, 0);
                Gl.glVertex3d(200, -10, 100);
                Gl.glVertex3d(125, -10, 100);
                Gl.glEnd();

                Gl.glPushMatrix();
                Gl.glColor3f(0.8f * lightIntensity, 0.8f * lightIntensity, 0.8f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(-25, -10, 75);
                Gl.glVertex3d(125, -10, 75);
                Gl.glVertex3d(125, -10, 100);
                Gl.glVertex3d(-25, -10, 100);
                Gl.glEnd();

                Gl.glPushMatrix();
                Gl.glColor3f(0.8f * lightIntensity, 0.8f * lightIntensity, 0.8f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(-25, -10, 0);
                Gl.glVertex3d(125, -10, 0);
                Gl.glVertex3d(125, -10, 25);
                Gl.glVertex3d(-25, -10, 25);
                Gl.glEnd();


                // стена левая
                Gl.glPushMatrix();
                Gl.glColor3f(0.8f * lightIntensity, 0.8f * lightIntensity, 0.8f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(-100, 200, 0);
                Gl.glVertex3d(200, 200, 0);
                Gl.glVertex3d(200, 200, 100);
                Gl.glVertex3d(-100, 200, 100);
                Gl.glEnd();

                // стена сзади с вырезом для двери
                Gl.glPushMatrix();
                Gl.glColor3f(0.8f * lightIntensity, 0.8f * lightIntensity, 0.8f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(-100, 200, 0);
                Gl.glVertex3d(-100, 65, 0);
                Gl.glVertex3d(-100, 65, 100);
                Gl.glVertex3d(-100, 200, 100);
                Gl.glEnd();

                Gl.glPushMatrix();
                Gl.glColor3f(0.8f * lightIntensity, 0.8f * lightIntensity, 0.8f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(-100, 65, 70);
                Gl.glVertex3d(-100, 65, 100);
                Gl.glVertex3d(-100, 30, 100);
                Gl.glVertex3d(-100, 30, 70);
                Gl.glEnd();

                Gl.glPushMatrix();
                Gl.glColor3f(0.8f * lightIntensity, 0.8f * lightIntensity, 0.8f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(-100, 30, 100);
                Gl.glVertex3d(-100, -10, 100);
                Gl.glVertex3d(-100, -10, 0);
                Gl.glVertex3d(-100, 30, 0);
                Gl.glEnd();

                //Циферблат
                Gl.glPushMatrix();
                Gl.glTranslated(90, 199, 55);
                Gl.glColor3f(1f * lightIntensity, 1f * lightIntensity, 1f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(0, 0, 0);
                Gl.glVertex3d(0, 0, 15);
                Gl.glVertex3d(15, 0, 15);
                Gl.glVertex3d(15, 0, 0);
                Gl.glEnd();
                Gl.glPopMatrix();

                //Стрелки
                Gl.glPushMatrix();
                Gl.glTranslated(97.5, 197, 62.5);
                Gl.glColor3f(0, 0, 0);
                Gl.glLineWidth(1f);
                Gl.glRotatef(alpha, 0, 1, 0);
                Gl.glBegin(Gl.GL_LINES);
                Gl.glVertex3d(0, 0, 0);
                Gl.glVertex3d(0, 0, 3.5);
                Gl.glEnd();
                alpha += 12;
                Gl.glPopMatrix();

                Gl.glPushMatrix();
                Gl.glTranslated(97.5, 197, 62.5);
                Gl.glColor3f(0, 0, 0);
                Gl.glLineWidth(2f);
                Gl.glRotatef(beta, 0, 1, 0);
                Gl.glBegin(Gl.GL_LINES);
                Gl.glVertex3d(0, 0, 0);
                Gl.glVertex3d(0, 0, 2);
                Gl.glEnd();
                beta += 1;
                Gl.glPopMatrix();

                // Человечек
                Gl.glPushMatrix();

                // Голова
                Gl.glColor3f(1.0f * lightIntensity, 0.8f * lightIntensity, 0.6f * lightIntensity); // Телесный цвет
                Gl.glTranslated(0, 20, 55);
                Glut.glutSolidSphere(10, 20, 20); // Радиус 10, детализация 20x20

                // Туловище
                Gl.glPushMatrix();
                Gl.glTranslated(0, 0, -20); // Смещаем ниже головы
                Gl.glColor3f(1.0f * lightIntensity, 0.0f * lightIntensity, 0.0f * lightIntensity); // Красный цвет
                Gl.glScaled(2, 2, 2.5); // Удлиненный куб
                Glut.glutSolidCube(10); // Размер куба 10
                Gl.glPopMatrix();

                // Левая рука
                Gl.glPushMatrix();
                Gl.glTranslated(-13, 20, 30f); // Смещаем влево и вниз от туловища
                Gl.glColor3f(1.0f * lightIntensity, 0.8f * lightIntensity, 0.6f * lightIntensity);
                Gl.glScaled(0.5, 0.5, 3); // Удлиненный куб
                Glut.glutSolidCube(10);
                Gl.glPopMatrix();

                // Правая рука
                Gl.glPushMatrix();
                Gl.glTranslated(13, 20, 30f); // Смещаем вправо и вниз от туловища
                Gl.glColor3f(0.7f * lightIntensity, 0.7f * lightIntensity, 0.8f * lightIntensity);
                Gl.glScaled(0.5, 0.5, 3);
                Glut.glutSolidCube(10);
                Gl.glPopMatrix();

                // Левая нога
                Gl.glPushMatrix();
                Gl.glTranslated(-5, 20, 0); // Смещаем вниз от туловища
                Gl.glColor3f(1.0f * lightIntensity, 0.8f * lightIntensity, 0.6f * lightIntensity);
                Gl.glScaled(0.5, 0.5, 4);
                Glut.glutSolidCube(10);
                Gl.glPopMatrix();

                // Правая нога
                Gl.glPushMatrix();
                Gl.glTranslated(5, 20, 0);
                Gl.glColor3f(0.7f * lightIntensity, 0.7f * lightIntensity, 0.8f * lightIntensity);
                Gl.glScaled(0.5f, 0.5f, 4);
                Glut.glutSolidCube(10);
                Gl.glPopMatrix();

                Gl.glPopMatrix();

                Gl.glPopMatrix();
                Gl.glFlush();
                AnT.Invalidate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Переключаем флаг для серого цвета
            isGrayscale = !isGrayscale;

            // Перезагружаем текстуры с учётом нового состояния флага
            loadImage();
            Draw();
        }

        private void loadImage()
        {
            string[] imagePaths = { "texture3.jpg", "texture2.jpg", "texture1.jpg" };

            for (int i = 0; i < 3; i++)
            {
                Bitmap bmp = new Bitmap(imagePaths[i]);
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                // Если нужно преобразовать в черно-белое
                if (isGrayscale)
                {
                    bmp = ConvertToGrayscale(bmp);
                }

                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                  ImageLockMode.ReadOnly,
                                                  PixelFormat.Format32bppArgb);

                spriteTextures[i] = MakeGlTexture(Gl.GL_RGBA, bmpData.Scan0, bmp.Width, bmp.Height);
                bmp.UnlockBits(bmpData);
                bmp.Dispose();
            }

            textureIsLoad = true;
        }

        private Bitmap ConvertToGrayscale(Bitmap original)
        {
            // Создаем новое изображение того же размера
            Bitmap grayscaleBitmap = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    // Получаем текущий цвет пикселя
                    Color pixelColor = original.GetPixel(x, y);

                    // Вычисляем среднее значение для цвета пикселя для получения оттенка серого
                    int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);

                    // Применяем серый цвет к новому изображению
                    Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                    grayscaleBitmap.SetPixel(x, y, grayColor);
                }
            }

            return grayscaleBitmap;
        }

        private static uint MakeGlTexture(int Format, IntPtr pixels, int w, int h)
        {

            // идентификатор текстурного объекта
            uint texObject;

            // генерируем текстурный объект
            Gl.glGenTextures(1, out texObject);

            // устанавливаем режим упаковки пикселей
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

            // создаем привязку к только что созданной текстуре
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texObject);

            // устанавливаем режим фильтрации и повторения текстуры
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);

            // создаем RGB или RGBA текстуру
            switch (Format)
            {

                case Gl.GL_RGB:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

                case Gl.GL_RGBA:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

            }

            // возвращаем идентификатор текстурного объекта

            return texObject;

        }

        private Bitmap GenerateMandelbrotFractal(int width, int height, double minRe, double maxRe, double minIm, double maxIm)
        {
            Bitmap bitmap = new Bitmap(width, height);

            // Параметры фрактала
            double reFactor = (maxRe - minRe) / width;
            double imFactor = (maxIm - minIm) / height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double cRe = minRe + x * reFactor;
                    double cIm = minIm + y * imFactor;

                    double zRe = cRe, zIm = cIm;
                    int iteration = 0;
                    int maxIterations = 1000;

                    // Итерации для проверки, выходит ли точка за пределы
                    while (zRe * zRe + zIm * zIm <= 4 && iteration < maxIterations)
                    {
                        double zReTemp = zRe * zRe - zIm * zIm + cRe;
                        zIm = 2 * zRe * zIm + cIm;
                        zRe = zReTemp;

                        iteration++;
                    }

                    // Вычисление цвета пикселя
                    int colorValue = (int)(255 * (double)iteration / maxIterations);
                    Color pixelColor = Color.FromArgb(colorValue, colorValue, colorValue);

                    bitmap.SetPixel(x, y, pixelColor);
                }
            }

            return bitmap;
        }

        private void ApplyTexture(Bitmap bitmap)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                                 ImageLockMode.ReadOnly,
                                                 PixelFormat.Format32bppArgb);

            mGlTextureObject2 = MakeGlTexture(Gl.GL_RGBA, bmpData.Scan0, bitmap.Width, bitmap.Height);
            bitmap.UnlockBits(bmpData);

            textureIsLoad = true;
        }
    }
}
