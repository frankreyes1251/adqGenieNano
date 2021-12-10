using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adqGenieNano
{
    /// <summary>
    /// Esta clase permite establecer los valores iniciales de la adquisición para las camaras
    /// de la marca Sapera, al existir una conexión al dispositivo, almacena en variables locales
    /// los parametros del dispositivo conectado, tales como:
    /// *-nombre del dispositivo
    /// *-archivo de configuración y calibración
    /// *-número de indice del dispositivo
    /// </summary>
    public class MyAcquisitionParams
    {
        /// <summary>
        /// Variable tipo String, guarda el nombre del servidor o dispositivo conectado
        /// </summary>
        protected string m_ServerName;
        /// <summary>
        /// Variable tipo int, Cada recurso o dispositivo conectado, requiere un indice de identificación.
        /// el drive asigna de forma automatica este Indice, es relevante ya que
        /// **Dos dispositivos pueden llamarse igual, pero tener un Indice diferente**
        /// </summary>
        protected int m_ResourceIndex;
        /// <summary>
        /// Variable tipo String, guarda la ruta completa del archivo de configuración para el dispositivo en particular.
        /// </summary>
        protected string m_ConfigFileName;

        /// <summary>
        /// Constructor Generico
        /// </summary>
        public MyAcquisitionParams()
        {
            m_ServerName = "Nano-M2420_1";
            m_ResourceIndex = 0;
            //m_ConfigFileName = "C:\\Users\\proyecto vr\\Documents\\Visual Studio 2015\\Projects\\VideoIR\\VideoIR\\archivo_configuracion.ccf";
            m_ConfigFileName = "E:\\Documentos\\Visual Studio 2015\\Projects\\adqGenieNano\\adqGenieNano\\CameraConfigFiles\\T_Nano-M2420_nano1_triggerout.ccf";
            //m_ConfigFileName = "C:\\Users\\SIVIME\\Documents\\SaperaEjemplos\\T_Calibir_640_GigE_Default_Default.ccf";
        }

        /// <summary>
        /// Constructor especializado, construye el objeto de adquisición con parametros
        /// especificos de un dispositivo
        /// </summary>
        /// <param name="ServerName"></param>
        /// <param name="ResourceIndex"></param>
        public MyAcquisitionParams(string ServerName, int ResourceIndex)
        {
            m_ServerName = ServerName;
            m_ResourceIndex = ResourceIndex;
            m_ConfigFileName = "";
        }

        /// <summary>
        /// Constructor especializado con archivo de calibración, construye el objeto de adquisición con parametros
        /// especificos de un dispositivo
        /// </summary>
        /// <param name="ServerName"></param>
        /// <param name="ResourceIndex"></param>
        /// <param name="ConfigFileName"></param>
        public MyAcquisitionParams(string ServerName, int ResourceIndex, string ConfigFileName)
        {
            m_ServerName = ServerName;
            m_ResourceIndex = ResourceIndex;
            m_ConfigFileName = ConfigFileName;
        }

        /// <summary>
        /// Estructura get set para configurar el nombre de archivo
        /// </summary>
        public string ConfigFileName
        {
            get { return m_ConfigFileName; }
            set { m_ConfigFileName = value; }
        }

        /// <summary>
        /// Estructura get set para configurar el nombre del dispositivo
        /// </summary>
        public string ServerName
        {
            get { return m_ServerName; }
            set { m_ServerName = value; }
        }

        /// <summary>
        /// Estructura get set para configurar el indice del dispositivo
        /// </summary>
        public int ResourceIndex
        {
            get { return m_ResourceIndex; }
            set { m_ResourceIndex = value; }
        }


    }//fin de la clase
}
