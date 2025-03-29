using System.Drawing.Imaging;
using System.Media;
using System.Runtime.InteropServices;
using Tao.DevIl;
using Tao.FreeGlut;
using Tao.OpenGl;

namespace Varakin_Oleg_PRI_121_LR_7
{
    public class RainParticle
    {
        public float X, Y, Z;
        public float SpeedZ;
    }
    public partial class Form1 : Form
    {
        private Random rand = new Random();
        private List<RainParticle> rainParticles = new List<RainParticle>();
        private bool isRaining = false;
        private SoundPlayer rainSound;
        double cubeX = 50;
        double cubeY = 100;
        double cubeZ = 0.0;
        double splineAngle = 0;
        double[] splineControlPoints = { 0, 30, 60, 90 };
        double splineTime = 0;
        double splineSpeed = 0.05;
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
            rainSound = new SoundPlayer("rain_sound.wav");
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
            Bitmap fractalBitmap = GenerateFernFractal(500, 500, 10000);
            ApplyTexture(fractalBitmap);
            RenderTimer.Start();
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            currentTextureIndex = (currentTextureIndex + 1) % spriteTextures.Length;
            if (isRaining)
            {
                UpdateRain();
            }
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

            if (e.KeyCode == Keys.H)
            {
                cubeY += 1;
            }
            if (e.KeyCode == Keys.K)
            {
                cubeY -= 1;
            }
            if (e.KeyCode == Keys.J)
            {
                cubeX -= 1;
            }
            if (e.KeyCode == Keys.U)
            {
                cubeX += 1;
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

                //Дождь
                if (isRaining)
                {
                    Gl.glPointSize(2.0f);
                    Gl.glBegin(Gl.GL_POINTS);
                    Gl.glColor3f(0.0f, 0.0f, 1.0f);
                    foreach (var particle in rainParticles)
                    {
                        Gl.glVertex3f(particle.X, particle.Y, particle.Z);
                    }
                    Gl.glEnd();
                }

                //Земля
                Gl.glColor3f(0.1f * lightIntensity, 0.1f * lightIntensity, 0.1f * lightIntensity);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3d(1000, 500, 0);
                Gl.glVertex3d(-1000, 500, 0);
                Gl.glVertex3d(-1000, -500, 0);
                Gl.glVertex3d(1000, -500, 0);
                Gl.glEnd();

                //Солнце
                Gl.glPushMatrix();
                Gl.glTranslated(500, 500, 400);
                Gl.glColor3f(1.0f * lightIntensity, 1.0f * lightIntensity, 0.0f * lightIntensity);
                Gl.glRotatef((float)splineAngle, 0, 1, 0);
                Glut.glutSolidCube(100);
                Gl.glPopMatrix();

                splineTime += splineSpeed;
                splineAngle = cubicSpline(splineTime, splineControlPoints);

                //Башмаки
                Gl.glPushMatrix();
                Gl.glTranslated(cubeX, cubeY, cubeZ);
                Gl.glColor3f(0.0f, 0.0f, 1.0f * lightIntensity);
                Glut.glutSolidCube(20);
                Gl.glPopMatrix();

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
                    bmp = ConvertToGrayscaleWithBlur(bmp);
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

        private Bitmap ConvertToGrayscaleWithBlur(Bitmap original)
        {
            // Создаем новое изображение того же размера
            Bitmap grayscaleBitmap = new Bitmap(original.Width, original.Height);

            // Создаем массивы пикселей для исходного изображения и для нового изображения
            BitmapData originalData = original.LockBits(new Rectangle(0, 0, original.Width, original.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData grayscaleData = grayscaleBitmap.LockBits(new Rectangle(0, 0, original.Width, original.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int width = original.Width;
            int height = original.Height;
            int stride = originalData.Stride;
            IntPtr originalScan0 = originalData.Scan0;
            IntPtr grayscaleScan0 = grayscaleData.Scan0;

            // Работаем с массивами байтов для ускорения обработки
            byte[] originalBytes = new byte[height * stride];
            byte[] grayscaleBytes = new byte[height * stride];

            Marshal.Copy(originalScan0, originalBytes, 0, originalBytes.Length);

            // Применяем размытие и преобразование в оттенки серого
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Индекс пикселя в массиве байтов
                    int pixelIndex = (y * stride) + (x * 4);

                    // Получаем текущий цвет пикселя
                    byte b = originalBytes[pixelIndex];
                    byte g = originalBytes[pixelIndex + 1];
                    byte r = originalBytes[pixelIndex + 2];

                    // Вычисляем оттенок серого
                    int grayValue = (int)(r * 0.3 + g * 0.59 + b * 0.11);

                    // Применяем размытие с учетом соседних пикселей
                    int blurValue = ApplyBlur(originalBytes, x, y, width, height, stride);

                    // Применяем серый цвет и эффект размытия к новому изображению
                    grayscaleBytes[pixelIndex] = (byte)blurValue;
                    grayscaleBytes[pixelIndex + 1] = (byte)blurValue;
                    grayscaleBytes[pixelIndex + 2] = (byte)blurValue;
                    grayscaleBytes[pixelIndex + 3] = 255; // Альфа-канал (полностью непрозрачный)
                }
            }

            // Копируем измененные байты обратно в изображение
            Marshal.Copy(grayscaleBytes, 0, grayscaleScan0, grayscaleBytes.Length);

            // Разблокируем память
            original.UnlockBits(originalData);
            grayscaleBitmap.UnlockBits(grayscaleData);

            return grayscaleBitmap;
        }

        private int ApplyBlur(byte[] imageBytes, int x, int y, int width, int height, int stride)
        {
            int blurRadius = 3; // Радиус размытия
            int blurSum = 0;
            int count = 0;

            // Проходим по ближайшим пикселям вокруг текущего
            for (int dy = -blurRadius; dy <= blurRadius; dy++)
            {
                for (int dx = -blurRadius; dx <= blurRadius; dx++)
                {
                    int neighborX = x + dx;
                    int neighborY = y + dy;

                    // Убедимся, что соседний пиксель внутри границ изображения
                    if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                    {
                        // Индекс соседнего пикселя в массиве байтов
                        int neighborIndex = (neighborY * stride) + (neighborX * 4);

                        // Получаем цвет соседнего пикселя
                        byte b = imageBytes[neighborIndex];
                        byte g = imageBytes[neighborIndex + 1];
                        byte r = imageBytes[neighborIndex + 2];

                        // Вычисляем оттенок серого
                        int neighborGrayValue = (int)(r * 0.3 + g * 0.59 + b * 0.11);

                        blurSum += neighborGrayValue;
                        count++;
                    }
                }
            }

            // Возвращаем усредненное значение для размытого пикселя
            return (int)(blurSum / count);
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

        private Bitmap GenerateFernFractal(int width, int height, int maxIterations)
        {
            Bitmap bitmap = new Bitmap(width, height);

            // Центрируем фрактал в центре изображения
            double x = 0, y = 0;

            Random random = new Random();

            // Итерации
            for (int i = 0; i < maxIterations; i++)
            {
                // Рандомно выбираем одно из четырех отображений
                double nextX = 0, nextY = 0;
                double rand = random.NextDouble();

                if (rand < 0.01) // 1% случаев
                {
                    nextX = 0;
                    nextY = 0.16 * y;
                }
                else if (rand < 0.86) // 85% случаев
                {
                    nextX = 0.85 * x + 0.04 * y;
                    nextY = -0.04 * x + 0.85 * y + 1.6;
                }
                else if (rand < 0.93) // 7% случаев
                {
                    nextX = 0.2 * x - 0.26 * y;
                    nextY = 0.23 * x + 0.22 * y + 1.6;
                }
                else // 7% случаев
                {
                    nextX = -0.15 * x + 0.28 * y;
                    nextY = 0.26 * x + 0.24 * y + 0.44;
                }

                x = nextX;
                y = nextY;

                // Преобразуем координаты в пиксели
                int px = (int)(width / 2 + x * width / 10); // Масштабируем
                int py = (int)(height - (y * height / 12)); // Масштабируем и переворачиваем по оси Y

                if (px >= 0 && px < width && py >= 0 && py < height)
                {
                    // Цвет пикселя (можно сделать его более красивым, например, через градиент)
                    bitmap.SetPixel(px, py, Color.Green);  // Используем зеленый цвет для фрактала
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

        double cubicSpline(double t, double[] controlPoints)
        {
            // Интерполяция по кубическому сплайну
            double t2 = t * t;
            double t3 = t2 * t;
            return controlPoints[0] * (1 - 3 * t2 + 2 * t3) +
                   controlPoints[1] * (3 * t2 - 2 * t3) +
                   controlPoints[2] * (t - 2 * t2 + t3) +
                   controlPoints[3] * (-t2 + t3);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isRaining = !isRaining;
            if (isRaining)
            {
                rainSound.PlayLooping();
                GenerateRainParticles();
            }
            else
            {
                rainSound.Stop();
                rainParticles.Clear();
            }
        }

        private void GenerateRainParticles()
        {
            rainParticles.Clear();
            for (int i = 0; i < 5000; i++)
            {
                float x, y;
                do
                {
                    x = (float)(rand.NextDouble() * 1000 - 500);
                    y = (float)(rand.NextDouble() * 1000 - 500);
                } while (x > -100 && x < 200 && y > -10 && y < 200);

                rainParticles.Add(new RainParticle()
                {
                    X = x,
                    Z = (float)(rand.NextDouble() * 300 + 100),
                    Y = y,
                    SpeedZ = (float)(rand.NextDouble() * 4 + 10)
                });
            }
        }

        private void UpdateRain()
        {
            foreach (var particle in rainParticles)
            {
                particle.Z -= particle.SpeedZ;
                if (particle.Z < 0)
                {
                    float x, y;
                    do
                    {
                        x = (float)(rand.NextDouble() * 1000 - 500);
                        y = (float)(rand.NextDouble() * 1000 - 500);
                    } while (x > -100 && x < 200 && y > -10 && y < 200);

                    particle.X = x;
                    particle.Y = y;
                    particle.Z = 500;
                }
            }
        }
    }
}