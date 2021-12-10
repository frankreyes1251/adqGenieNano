using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALSA.SaperaLT.SapClassBasic;
using System.Windows.Forms;

namespace adqGenieNano
{
    /// <summary>
    /// Esta clase permite establecer los valores de procesmiento de la cámara,
    /// tales como el número de bits de la imagene, el tamaño en pixels, 
    /// pixel caliente es blanco o negro.
    /// Existe un ejemplo de sapera en donde se consultan todos los "features"
    /// C:\Program Files\Teledyne DALSA\Sapera\Examples\NET\CameraFeatures
    /// </summary>
    class DeviceFeatures
    {
        Boolean isAvailable = false;
        SapFeature feature;
        SapAcqDevice device;
        /// <summary>
        /// Constructor principal vacio
        /// </summary>
        public DeviceFeatures()
        {

        }
        /// <summary>
        /// Segundo Construtor de la clase, se encarga de recibir el objeto dispositivo,
        /// también recibe el nombre del feature que se desea modificar
        /// </summary>
        /// <param name="m_device"></param>
        /// <param name="m_feature"></param>
        public DeviceFeatures(SapAcqDevice m_device, SapFeature m_feature)
        {
            this.device = m_device;
            this.feature = m_feature;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        


        /////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Cambia una propiedad de la adquisición de la imagen cuando la propiedad tiene formato "string"
        /// Retorna true si pudo cambiar la propiedad
        /// </summary>
        /// <param name="properti"></param>
        /// <param name="newProperti"></param>
        /// <returns name ="status" ></returns>
        public bool changeStringFeature(string properti, string newFeatureValue)
        {
            bool status = false;

            status = device.IsFeatureAvailable(properti);
            if (device.IsFeatureAvailable(properti))
            {
                isAvailable = true;
                if (device.GetFeatureInfo(properti, feature))
                    status = true;
            }//fin del if device is available

            if (status && isAvailable && feature.DataAccessMode == SapFeature.AccessMode.RW || status && isAvailable && feature.DataAccessMode == SapFeature.AccessMode.WO)
            {
                //Utiliza currentProperti para almacenar la propiedad actual
                String currentUserId;
                if (!device.IsEventEnabled("Feature Value Changed"))
                    device.EnableEvent("Feature Value Changed");

                // get DeviceUserID value from the device using the name of the feature
                if(feature.DataAccessMode == SapFeature.AccessMode.RW || feature.DataAccessMode == SapFeature.AccessMode.RO)
                    device.GetFeatureValue(properti, out currentUserId);


                // Set new value for DeviceUserID feature in device
                if (feature.DataAccessMode == SapFeature.AccessMode.RW || feature.DataAccessMode == SapFeature.AccessMode.WO)
                    device.SetFeatureValue(properti, newFeatureValue);

                //MessageBox.Show("Set DeviceUserID to " + newFeatureValue);

                // Unregister event by name
                if (device.IsEventEnabled("Feature Value Changed"))
                    device.DisableEvent("Feature Value Changed");

                // Restore DeviceUserID value to old value
                // device.SetFeatureValue("DeviceUserID", currentUserId);
                // MessageBox.Show("Set DeviceUserID to old value: {0}", currentUserId);
            }//fin del if status

            return status;
        }//fin del metodo changeStringFeature


        /// <summary>
        /// Cambia una propiedad de la adquisición de la imagen cuando la propiedad tiene formato "bool"
        /// Retorna true si pudo cambiar la propiedad
        /// </summary>
        /// <param name="properti"></param>
        /// <param name="newProperti"></param>
        /// <returns name ="status" ></returns>
        public bool changeBooleanFeature(string properti, bool newFeatureValue)
        {
            bool status = false;

            if (device.IsFeatureAvailable(properti))
            {
                isAvailable = true;
                if (device.GetFeatureInfo(properti, feature))
                    status = true;
            }//fin del if device is available

            if (status && isAvailable && feature.DataAccessMode == SapFeature.AccessMode.RW || status && isAvailable && feature.DataAccessMode == SapFeature.AccessMode.WO)
            {
                try
                {
                    //Utiliza currentProperti para almacenar la propiedad actual
                    Boolean oldFeatureValue;
                if (!device.IsEventEnabled("Feature Value Changed"))
                    device.EnableEvent("Feature Value Changed");

               //lee el valor actual de la propiedad cuado el leible
                if(feature.DataAccessMode == SapFeature.AccessMode.RW || feature.DataAccessMode == SapFeature.AccessMode.RO)
                    device.GetFeatureValue(properti, out oldFeatureValue);


                    // Set new value for DeviceUserID feature in device
                if (feature.DataAccessMode == SapFeature.AccessMode.RW || feature.DataAccessMode == SapFeature.AccessMode.WO)
                    device.SetFeatureValue(properti, newFeatureValue);
               
                //MessageBox.Show("Set DeviceUserID to white Hot = " + whiteHot.ToString());

                // Unregister event by name
                if (device.IsEventEnabled("Feature Value Changed"))
                    device.DisableEvent("Feature Value Changed");
                }
                catch { }
                // Restore DeviceUserID value to old value
                // device.SetFeatureValue("DeviceUserID", currentUserId);
                // MessageBox.Show("Set DeviceUserID to old value: {0}", currentUserId);
            }//fin del if status

            return status;
        }//fin del metodo changeBooleanFeature

        /// <summary>
        /// Cambia especificamente la propiedad del Formato de Pixel, puede ser mono 8, mono 10, mono 14.
        /// Retorna true si pudo cambiar la propiedad
        /// </summary>
        /// <param name="properti"></param>
        /// <param name="newProperti"></param>
        /// <returns name ="status" ></returns>
        public bool changePixelFormatFeature(string properti, string newFeatureValue)
        {
            bool status = false;

            if (device.IsFeatureAvailable(properti))
            {
                isAvailable = true;
                if (device.GetFeatureInfo(properti, feature))
                    status = true;
            }//fin del if device is available

            if (status && isAvailable && feature.DataAccessMode == SapFeature.AccessMode.RW)
            {
                //Utiliza currentProperti para almacenar la propiedad actual
                string actualPixelFormat;
                if (!device.IsEventEnabled("Feature Value Changed"))
                    device.EnableEvent("Feature Value Changed");

                // get DeviceUserID value from the device using the name of the feature
                device.GetFeatureValue(properti, out actualPixelFormat);


                // Set new value for DeviceUserID feature in device
                device.SetFeatureValue(properti, newFeatureValue);

                //MessageBox.Show("Set mono format de = " + actualPixelFormat +" a "+ newFeatureValue);

                // Unregister event by name
                if (device.IsEventEnabled("Feature Value Changed"))
                    device.DisableEvent("Feature Value Changed");

                // Restore DeviceUserID value to old value
                // device.SetFeatureValue("DeviceUserID", currentUserId);
                // MessageBox.Show("Set DeviceUserID to old value: {0}", currentUserId);
            }//fin del if status

            return status;
        }//fin del metodo changeBooleanFeature

        /// <summary>
        /// Cambia una propiedad de la adquisición de la imagen cuando la propiedad tiene formato "Int64"
        /// Retorna true si pudo cambiar la propiedad
        /// </summary>
        /// <param name="properti"></param>
        /// <param name="newProperti"></param>
        /// <returns name ="status" ></returns>
        public bool changeInt64Feature(string properti, long newFeatureValue)
        {
            bool status = true;
            status = device.IsFeatureAvailable(properti);
            if (status)
            {
                isAvailable = true;
                if (device.GetFeatureInfo(properti, feature))
                    status = true;

            }
            if (status && isAvailable && feature.DataAccessMode == SapFeature.AccessMode.RW)
            {
                try
                {
                    //Utiliza currentProperti para almacenar la propiedad actual
                    Int64 oldValue;
                    if (!device.IsEventEnabled("Feature Value Changed"))
                        device.EnableEvent("Feature Value Changed");

                    // get DeviceUserID value from the device using the name of the feature
                    device.GetFeatureValue(properti, out oldValue);


                    // Set new value for DeviceUserID feature in device
                    device.SetFeatureValue(properti, newFeatureValue);
                    status = true;
                    //MessageBox.Show("Set range conrast to newValueContraste = " + newValueContraste.ToString());

                    // Unregister event by name
                    if (device.IsEventEnabled("Feature Value Changed"))
                        device.DisableEvent("Feature Value Changed");
                }
                catch { }
                // Restore DeviceUserID value to old value
                // device.SetFeatureValue("DeviceUserID", currentUserId);
                // MessageBox.Show("Set DeviceUserID to old value: {0}", currentUserId);
            }//fin del if status
            return status;
        }//fin del metodo changeInt64Feature


    }//fin de la clase
}
