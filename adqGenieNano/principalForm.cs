using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

using Emgu.CV;
using Emgu.CV.Structure;
using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.SapClassGui;
using adqGenieNano.Elements;


namespace adqGenieNano
{
    public partial class principalForm : Form
    {
      
        SapFeature m_feature = null;
        MyAcquisitionParams m_Params;
        //SapAcqDevice m_AcqDevice;
        //static SapBuffer m_Buffers;
        //SapAcqDeviceToBuf m_Xfer;
        DeviceFeatures devFeature;
        public bool statusPlay = false;
        static Bitmap img_aux;
        static Mat a;
        // lista que contiene algunos filtros
        static List<ConvolutionFilterBase> filterList;
        //UpDown Treshold
        Int64 newValueTreshold;
        bool objects = false;
        static Image<Gray, byte> I;

        // ---  Nuestros datos -- ELY
        static byte[,] Data;
        static double[,] Mag_Grad;

        public static int nr, nc;
        //  ----  FIN de declaración de nuestros datos  ELY ----

        /// <summary>
        /// Constructor de la clase principalForm
        /// </summary>
        public principalForm()
        {
            AcqConfigDlg acConfigDlg = new AcqConfigDlg(null, "", AcqConfigDlg.ServerCategory.ServerAcqDevice);
            if (acConfigDlg.ShowDialog() == DialogResult.OK)
            {
                InitializeComponent();

                m_AcqDevice = null;
                m_Buffers = null;
                m_Xfer = null;
                m_Params = new MyAcquisitionParams();

                this.newValueTreshold = 0;
                
                //objects = CreateNewObjects(m_Params.ServerName, m_Params.ResourceIndex);//Creación de objetos de Sapera SIN uso de archivo de configuración de camara

                
                objects = CreateNewObjects(acConfigDlg, false);//Creación de objetos de Sapera CON uso de archivo de configuración de camara
                if (objects)
                {
                    //bool featureCreado = m_feature.Create();
                    //string propertiID = "DeviceUserID";
                    //string newValueID = "Genie-Nano";
                    //devFeature = new DeviceFeatures(m_AcqDevice, m_feature);
                    //bool estadoID = devFeature.changeStringFeature(propertiID, newValueID);
                }//fin del if verificación de  objetos creados
                else
                {
                    this.Close();
                }

            }//fin del if acConfigDlg
            else
            {
                MessageBox.Show("No GigE-Vision cameras found or selected");
                this.Close();
            }
        }//Fin del constructor pricipalForm()

        /// <summary>
        /// xfer_XferNotify Es para la cámara IR, establece la imagen en el pictureBox1
        /// e identifica que filtro fue seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="argsNotify"></param>
        static public void xfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
        {
            principalForm GigeDlg = argsNotify.Context as principalForm;
            if (argsNotify.Trash)
            {
                return;
            }
            else
            {
                Graphics gBox = GigeDlg.mainPictureBox.CreateGraphics();
                Graphics IBox = GigeDlg.pictureBoxImageTest.CreateGraphics();
                Int64 threshold = GigeDlg.newValueTreshold;
                IntPtr addr;
                //PixelFormat pixFormat = PixelFormat.Format8bppIndexed;//
                //PixelFormat pixFormat = PixelFormat.Format1bppIndexed;//
                //PixelFormat pixFormat = PixelFormat.Format4bppIndexed;//
                //PixelFormat pixFormat = PixelFormat.Format4bppIndexed;//
                //ConvolutionFilterBase filter = filterList[0];
                //creacion del buffer de la camara para adquirir la imagen
                m_Buffers.GetAddress(out addr);
                
                //creación de objeto bitmap para ser manipulado
                //se construye el bitmapcon base a la info de la dirección del buffer en addr
                img_aux = new Bitmap(m_Buffers.Width, m_Buffers.Height, m_Buffers.Pitch, PixelFormat.Format8bppIndexed, addr);
                //img_aux.Palette = GetGrayScalePalette();
                a = img_aux.ToMat();
                
                Bitmap img_aux1 = img_aux;
                //img_aux1 = new Bitmap(img_aux, img_aux.Width / 2, img_aux.Height / 2);
                Bitmap img_aux2 = img_aux1;

               //buff2Data2D(img_aux); // paso los datos de la imagen a la var Data   --- ELY
               //CalculaMagGrad(); // calcular la magnitud del gradiente   -- ELY


                img_aux2 = AplicarThreshold_AsArray(img_aux, threshold);
                
                img_aux2.Palette = GetGrayScalePalette();
                Bitmap img_aux3 = img_aux2;
                gBox.DrawImage(img_aux3, 0, 0, GigeDlg.mainPictureBox.Width, GigeDlg.mainPictureBox.Height);
                //IBox.DrawImage(CovertirImagen(img_aux1).ToBitmap(), 0,0, GigeDlg.mainPictureBox.Width, GigeDlg.mainPictureBox.Height);
                
                gBox.Dispose();
                IBox.Dispose();
            }
        }

        /// <summary>
        /// Filtro para cambiar la paleta de colores
        /// </summary>
        /// <returns></returns>
        
        static ColorPalette GetGrayScalePalette()
        {
            Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
            ColorPalette monoPalette = bmp.Palette;
            Color[] entries = monoPalette.Entries;
            int m = 1;
            Parallel.For(0, entries.Length, i => monoPalette.Entries[i] = Color.FromArgb(i / m, i / m, i / m));
            return monoPalette;
        }

        //  ELY  --
        // Función para pasar los datos de la imagen que están en un buffer lineal a matriz 2D e inicializar otras matrices de una vez 
        static public void buff2Data2D (Bitmap sourceBitmap)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);  // enfoque en datos de la imagen, cada pxl es de 8 bits
            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];   // el tamaño del ren * el número de cols
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);     // pixelBuffer <- sourceData (y sourceData contiene datos de sourceBitmap)
            sourceBitmap.UnlockBits(sourceData);

            nr = sourceBitmap.Height;
            nc = sourceBitmap.Width;

            Data = new byte[nr, nc];
            Mag_Grad = new double [nr, nc];

            int ren=0, col=0;           
            for (int k = 0; k < pixelBuffer.Length; k++)
            {
                Data[ren, col] = pixelBuffer[k];    // coloco dato de la imagen
                Mag_Grad[ren, col] = 0;             // inicializo en ceros

                if (col == nc - 1) // ¿llegué al final del renglón?
                {
                    col = 0;
                    ren++;
                }
                else
                    col++;
            }
            return ;      
        }

        // ELY  --
        // Función que calcula la magnitud del gradiente, considerando que los datos están en la matriz Data
        static public void CalculaMagGrad()
        {
            for (int i = 1; i < nr - 2; i++)
                for (int j = 1; j < nc - 2; j++)
                {
                    Mag_Grad[i, j] = Math.Sqrt( Math.Pow(Data[i,j],2) - 2*Data[i,j]*Data[i,j-1] + Math.Pow(Data[i, j-1], 2) + 
                                                Math.Pow(Data[i,j],2) - 2*Data[i,j]*Data[i-1,j] + Math.Pow(Data[i-1,j], 2) ) ; 
                }
        }
        

        static public Image<Rgb, Byte> CovertirImagen(Bitmap sourceBitmap)
        {
            Image<Rgb, Byte> emguImage = sourceBitmap.ToImage<Rgb, Byte>();

            return emguImage;
        }

        /// <summary>
        /// Filtro utilizado array de pixelBuffer
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static Bitmap AplicarThreshold_AsArray(Bitmap sourceBitmap, Int64 threshold)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);
            double contrastLevel = Math.Pow((100.0 + threshold) / 100.0, 2);
            double gray = 0;
            byte actualpixel = 0;         
            for (int k = 0; k< pixelBuffer.Length; k++)
            {
                actualpixel = pixelBuffer[k];
                gray = ((((pixelBuffer[k]/255) - 0.4) * contrastLevel) + 0.2) * 180.0;                
                //Gray rules
                if (gray > 255)
                { gray = 255; }
                else if (gray < 0)
                { gray = 0; }                
                pixelBuffer[k] = (byte)gray;
            }
            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            resultBitmap.Palette = GetGrayScalePalette();
            return resultBitmap;
        }//fin del metodo AplicarThreshold_AsArray

        /// <summary>
        /// Filtro utilizado array de pixelBuffer
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static Bitmap AplicarResta(Bitmap sourceBitmap, Int64 threshold)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);
            double contrastLevel = Math.Pow((100.0 + threshold) / 100.0, 2);
            double gray = 0;
            double actualpixel = 0;
            gray = (double)pixelBuffer[0] - actualpixel;
            for (int k = 0; k < pixelBuffer.Length; k++)
            {
                actualpixel = (double)pixelBuffer[k];
                gray = (((((double)pixelBuffer[k] / 255.0) - 0.4) * contrastLevel) + 0.2) * 180.0;
                //Gray rules
                if (gray > 255)
                { gray = 255; }
                else if (gray < 0)
                { gray = 0; }
                pixelBuffer[k] = (byte)gray;
            }
            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            resultBitmap.Palette = GetGrayScalePalette();
            return resultBitmap;
        }//fin del metodo AplicarThreshold_AsArray

        /// <summary>
        /// Filtro utilizado array de pixelBuffer
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        /*public static Bitmap AplicarThreshold_AsMatrix(Bitmap sourceBitmap, Int64 threshold)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            byte[][] pixelBuffer = new byte[sourceData.Stride][sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);
            double contrastLevel = Math.Pow((100.0 + threshold) / 100.0, 2);
            double gray = 0;
            double green = 0;
            double red = 0;
            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                gray = ((((pixelBuffer[k] / 255.0) - 0.4) * contrastLevel) + 0.2) * 180.0;
                //green = ((((pixelBuffer[k + 1] / 255.0) - 0.4) * contrastLevel) + 0.2) * 180.0;
                //red = ((((pixelBuffer[k + 2] / 255.0) - 0.4) * contrastLevel) + 0.2) * 180.0;
                //Gray rules
                if (gray > 255)
                { gray = 255; }
                else if (gray < 0)
                { gray = 0; }
                /*
                if (green > 255)
                { green = 255; }
                else if (green < 0)
                { green = 0; }
                if (red > 255)
                { red = 255; }
                else if (red < 0)
                { red = 0; }
                pixelBuffer[k] = (byte)gray;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;
            }
            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
            ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            return resultBitmap;
        }*/

        /// <summary>
        /// Crea los objetos necesarios para la adquisición del buffer de la camara, 
        /// a partir de una lista de camaras sin usar un archivo de configuración
        /// </summary>
        /// <param name="m_ServerName"></param>
        /// <param name="m_ResourceIndex"></param>
        /// <returns></returns>
        public bool  CreateNewObjects(string m_ServerName, int m_ResourceIndex)
        {
            ArrayList listServerNames = new System.Collections.ArrayList();
            int serverCount = SapManager.GetServerCount();
            
            bool cameraFound = false;
            string serverName;
            int serverIndex;
            if (serverCount == 0)
            {
                MessageBox.Show("No se encontro ninguna cámara Calibir IR!");
                return false;
            }
            else
            {
                for (serverIndex = 0; serverIndex < serverCount; serverIndex++)
                {
                    serverName = SapManager.GetServerName(serverIndex);
                    listServerNames.Add(serverName);
                    if (serverName == m_ServerName) cameraFound = true;
                    else cameraFound = false;
                }
                if (cameraFound)
                {
                    m_ServerLocation = new SapLocation(m_ServerName, m_ResourceIndex);
                    m_AcqDevice = new SapAcqDevice(m_ServerLocation, "");//recive el nombre del servidor local y la ruta del archivo con nombre
                    bool creacionAcqDev = m_AcqDevice.Create();
                    /*m_feature = new SapFeature(location);
                    bool featureCreado = m_feature.Create();
                    devFeature = new DeviceFeatures(m_AcqDevice, m_feature);
                    string propertiPixFormat = "PixelFormat";
                    string newValuePixFormat = "Mono8";
                    bool estadoPixelFormat = devFeature.changePixelFormatFeature(propertiPixFormat, newValuePixFormat);
                    string propertiCalibracion = "flatfieldCalibrationFPN";
                    bool calibracion = true;
                    bool esatdoCalibracion = devFeature.changeBooleanFeature(propertiCalibracion, calibracion);
                    string propertiSaveCalibracion = "flatfieldCalibrationSave";
                    bool saveCalibracion = true;
                    bool estadoSaveCalibracion = devFeature.changeBooleanFeature(propertiSaveCalibracion, saveCalibracion);
                    string propertiCalibracionLente = "lensCorrection";
                    string calibracionLente = "H1";
                    bool estadoCalibracionLente = devFeature.changeStringFeature(propertiCalibracionLente, calibracionLente);
                    //IEnumerable calibracionLente = */
                    try
                    {
                        bool buferSoportado = SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather);
                        if (buferSoportado)
                            m_Buffers = new SapBufferWithTrash(2, m_AcqDevice, SapBuffer.MemoryType.ScatterGather);
                        else
                            m_Buffers = new SapBufferWithTrash(2, m_AcqDevice, SapBuffer.MemoryType.ScatterGatherPhysical);
                    }
                    catch { }
                    m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers);
                    m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
                    m_Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
                    m_Xfer.XferNotifyContext = this;
                    if (!CreateObjects())
                    {
                        DisposeObjects();
                        return false;
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("No se encontro la Cámara seleccionada CalibIR: " + m_ServerName);
                    return false;
                }
            }
        }

        /// <summary>
        /// Crea los objetos necesarios para la adquisición del buffer de la camara, 
        /// a partir de una lista de camaras y su archivo de configuración.
        /// </summary>
        /// <param name="acConfigDlg"></param>
        /// <param name="Restore"></param>
        /// <returns></returns>
        public bool CreateNewObjects(AcqConfigDlg acConfigDlg, bool Restore)
        {
            if (!Restore)
            {
                m_ServerLocation = acConfigDlg.ServerLocation;
                m_ConfigFileName = acConfigDlg.ConfigFile;
            }
            m_AcqDevice = new SapAcqDevice(m_ServerLocation, m_ConfigFileName);//recive el nombre del servidor local y la ruta del archivo con nombre
            if (SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather))
                m_Buffers = new SapBufferWithTrash(2, m_AcqDevice, SapBuffer.MemoryType.ScatterGather);
            else
                m_Buffers = new SapBufferWithTrash(2, m_AcqDevice, SapBuffer.MemoryType.ScatterGatherPhysical);
            m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers);
           

            m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            m_Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
            m_Xfer.XferNotifyContext = this;
            

            if (!CreateObjects())
            {
                DisposeObjects();
                return false;
            }

            return true;
        }


        /// <summary>
        /// Llama a los objetos creados y verifica que haya sido inicializada la camara
        /// Este metodo es para la camara de Dalsa IR
        /// </summary>
        /// <returns></returns>
        public bool CreateObjects()
        {
            if (m_AcqDevice != null && !m_AcqDevice.Initialized)
            {
                if (m_AcqDevice.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }
            if (m_Buffers != null && !m_Buffers.Initialized)
            {
                if (m_Buffers.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
                m_Buffers.Clear();
            }
            if (m_Xfer != null && m_Xfer.Pairs[0] != null)
            {
                m_Xfer.Pairs[0].Cycle = SapXferPair.CycleMode.NextWithTrash;
                if (m_Xfer.Pairs[0].Cycle != SapXferPair.CycleMode.NextWithTrash)
                {
                    DestroyObjects();
                    return false;
                }
            }
            if (m_Xfer != null && !m_Xfer.Initialized)
            {
                if (m_Xfer.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }
            return true;
        }//fin del metodo create objets

        /*			 Destroy Object & Dispose Object
        ***************************************************/
        /// <summary>
        /// Destruye los objetos de busqueda de camara, creación de Buffer y Adquisición de dispositivo
        /// </summary>
        public void DestroyObjects()
        {
            if (m_Xfer != null && m_Xfer.Initialized)
                m_Xfer.Destroy();
            if (m_Buffers != null && m_Buffers.Initialized)
                m_Buffers.Destroy();
            if (m_AcqDevice != null && m_AcqDevice.Initialized)
                m_AcqDevice.Destroy();
        }
        /// <summary>
        /// Libera memoria de los objetos busqueda de camara, creación de Buffer y Adquisición de dispositivo
        /// </summary>
        public void DisposeObjects()
        {
            if (m_Xfer != null)
            {
                m_Xfer.Dispose();
                m_Xfer = null;
            }
            if (m_Buffers != null)
            {
                m_Buffers.Dispose();
                m_Buffers = null;
            }
            if (m_AcqDevice != null)
            {
                m_AcqDevice.Dispose();
                m_AcqDevice = null;
            }
        }

        /// <summary>
        /// Este método inicia la grabación de la cámara IR
        /// </summary>
        public void PlayIR()
        {
            #region Inizializa  la camara IR(Teledyne) Infrarroja.
            statusPlay = true;
            if (statusPlay == true)
            {
                try
                {
                    m_Xfer.Grab();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error al Adquirir Imagen Dalsa", "Hubo problemas al iniciar la camara: " + ex.ToString());

                }
            }
            #endregion
        }
        /// <summary>
        /// Detiene la adquisición de imágenes de la cámara IR
        /// </summary>
        public void PauseIR()
        {
            try
            {
                #region Pausa o stop para la camara IR(Teledyne)
                if (statusPlay == true)
                    if (m_Xfer.Freeze())
                    {
                        m_Xfer.Abort();
                        statusPlay = false;
                    }
                    else
                        MessageBox.Show("No hay cámara activada");
                #endregion
            }
            catch { }
        }// fin de pausar camara

        /// <summary>
        /// verificarCamaraIR verifica que la cámara IR no este adquiriendo imagenes antes de iniciar nuevamente
        /// la adquisición de los fotogramas.
        /// </summary>
        public void verificarCamaraIR()
        {
            try
            {
                if (statusPlay == true)
                {
                    PauseIR();
                    statusPlay = false;
                    // Lo que se encuentra a continuación es para limpiar el pictureBox1.
                    mainPictureBox.Image = null;
                    mainPictureBox.Invalidate();
                    if (statusPlay == false)
                    {
                        PlayIR();
                        statusPlay = true;
                        //errorCamara.Text = "";
                    }
                }
                else
                {
                    try
                    {
                        PlayIR();
                        statusPlay = true;
                        //errorCamara.Text = "";
                    }
                    catch { }
                }
            }
            catch { }
        }

        /// <summary>
        /// Boton para adquisición de un frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SingleFrame_Click(object sender, EventArgs e)
        {

            verificarCamaraIR();

        }//fin del metodo button_SingleFrame_Clic

        private void button_StopFrameAdq_Click(object sender, EventArgs e)
        {
            PauseIR();
        }

        private void numericUpDown_Treshold_ValueChanged(object sender, EventArgs e)
        {
            this.newValueTreshold = Decimal.ToInt64(this.numericUpDown_Treshold.Value);

        }
    }//fin de la clase
}//fin del espacio de nombres


/*
 A static method in C# is a method that keeps only one copy of the method at the Type level, not the object level. 
 That means, all instances of the class share the same copy of the method and its data. The last updated value of 
 the method is shared among all objects of that Type.
 
     
     */
