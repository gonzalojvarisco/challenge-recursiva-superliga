using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GonzaloVariscoCSV
{
    class Program
    {
        static List<Socio> socios = new List<Socio>();
        static int countSociosRacing = 0;
        static int edadSociosRacing = 0;
        static List<Socio> sociosCasadosUniv = new List<Socio>();
        static Dictionary<string, int> nombresComunes = new Dictionary<string, int>();
        static Dictionary<string, EstadisticaEquipo> estadisticasEquipos = new Dictionary<string, EstadisticaEquipo>();
        static void Main(string[] args)
        {
            // 1-- obtener coleccion de socios que hay en el archivo socios.csv
            cargarSocios("socios.csv");

            // 2-- procesar una a una las filas y llevar unas variables de totales donde ir 
            //     acumulando lo que se pide.
            foreach (Socio unSocio in socios)
            {
                //-- acumulo para sacar el promedio de edad de racing
                if (unSocio.equipo == "Racing")
                {
                    countSociosRacing++;
                    edadSociosRacing += unSocio.edad;
                }

                //-- acumulo socios casados universitarios
                if (unSocio.estadoCivil == "Casado" &&
                     unSocio.estudios == "Universitario" &&
                     sociosCasadosUniv.Count < 100)
                {
                    sociosCasadosUniv.Add(unSocio);
                }

                //-- nombres comunes en River
                if (unSocio.equipo == "River")
                {
                    if (nombresComunes.ContainsKey(unSocio.nombre))
                    {
                        nombresComunes[unSocio.nombre]++;
                    }
                    else
                    {
                        nombresComunes[unSocio.nombre] = 1;
                    }
                }

                //-- estadisticas por equipo
                EstadisticaEquipo e;
                if (!estadisticasEquipos.ContainsKey(unSocio.equipo))
                {
                    e = new EstadisticaEquipo();
                    e.nombre = unSocio.equipo;
                    estadisticasEquipos.Add(unSocio.equipo, e);
                }
                else
                {
                    e = estadisticasEquipos[unSocio.equipo];
                }

                e.cantSocios++;
                e.sumaEdad += unSocio.edad;
                if (e.mayorEdad < unSocio.edad)
                {
                    e.mayorEdad = unSocio.edad;
                }
                if (e.menorEdad > unSocio.edad)
                {
                    e.menorEdad = unSocio.edad;
                }

            }


            // 3-- hacer calculos sobre las variables que fuimos acumulando

            // 4-- mostrar los totales
            mostrarResultados();
        }

        static void
        mostrarResultados()
        {
            // 1--
            Console.WriteLine("Total personas registradas: " + socios.Count.ToString());

            // 2--
            Console.WriteLine("Promedio edad racing: " + (edadSociosRacing / countSociosRacing).ToString());

            // 3--
            Console.WriteLine("SOCIOS CASADOS UNIVERSITARIOS:");
            var items = from Socio unSocio
                        in sociosCasadosUniv
                        orderby unSocio.edad
                        select unSocio;
            foreach (Socio unSocio in items)
            {
                Console.WriteLine(unSocio.nombre + ", " + unSocio.edad.ToString() + " años, de " + unSocio.equipo);
            }

            // 4--
            Console.WriteLine("5 NOMBRES MAS COMUNES EN RIVER:");
            var items2 = (from t
                         in nombresComunes
                          orderby t.Value descending
                          select t).Take(5);
            foreach (KeyValuePair<string, int> item in items2)
            {
                Console.WriteLine(item.Key + ": " + item.Value.ToString() + " veces.");
            }

            // 5-- listado
            // recorrer el diccionario y mostrar los valores
            foreach (KeyValuePair<string, EstadisticaEquipo> item in estadisticasEquipos)
            {
                Console.WriteLine(item.Value.nombre + ": promedio " + item.Value.promedioEdad().ToString() + " años, menor " + item.Value.menorEdad.ToString() + " años, mayor " + item.Value.mayorEdad.ToString() + " años.");
            }

            Console.ReadLine();
        }

        static void cargarSocios(string archivo)
        {
            string[] filas;
            filas = System.IO.File.ReadAllLines(archivo);

            string[] columnas;
            char[] separadores = { ';' };

            foreach (string fila in filas)
            {
                columnas = fila.Split(separadores);

                Socio unSocio = new Socio();
                unSocio.nombre = columnas[0];
                unSocio.edad = Convert.ToInt32(columnas[1]);
                unSocio.equipo = columnas[2];
                unSocio.estadoCivil = columnas[3];
                unSocio.estudios = columnas[4];

                socios.Add(unSocio);
            }
        }
    }

    class Socio
    {
        public string nombre { get; set; }
        public int edad { get; set; }
        public string equipo { get; set; }
        public string estadoCivil { get; set; }
        public string estudios { get; set; }
    }

    class EstadisticaEquipo
    {
        public string nombre { get; set; }
        public int cantSocios { get; set; }
        public int sumaEdad { get; set; }
        public int menorEdad { get; set; } = 99;
        public int mayorEdad { get; set; }

        public float promedioEdad()
        {
            return sumaEdad / cantSocios;
        }
    }

}
