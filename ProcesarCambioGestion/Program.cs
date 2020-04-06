using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ProcesarCambioGestion
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }

        /// <summary>
        /// Metodo que ejecuta automaticamente las tareas siguientes:
        /// 1. Cambia el nombre de las listas de gestiones anteriores.
        /// 2. Crea nuevas listas a partir de plantillas existentes.
        /// 3. Reasigna permisos a las nuevas listas.
        /// 4. Elimina eventos repetidos en nuevas listas.
        /// </summary>
        /// <param name="sitio">URL del sitio objetivo</param>
        private static void ProcesoCambioDeGestion(string sitio)
        {
            try
            {
                

                Console.WriteLine("Operación completada exitosamente.");
            }
            catch (Exception ex)
            {
                #region Registro en log
                using (StreamWriter w = File.AppendText("log.txt"))
                {
                    Log("ERROR >> " + ex.Message, w);
                    w.Close();
                }
                #endregion

                Console.WriteLine("Se produjo un error. Consulte el archivo 'log.txt' generado.");
            }
        }

        #region Log
        private static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
            // Update the underlying file.
            w.Flush();
        }
        #endregion
    }
}
