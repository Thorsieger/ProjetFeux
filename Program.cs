using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmdCrf;
using System.Threading;
using System.IO;


namespace feux_test
{
    class Program
    {

        static void feux1(CmdFeux Fx)
        {
            Fx.LectureReseau();

            if (Fx.Feux1.Rouge == true)
            {
                Fx.Feux1.Rouge = false;
                Fx.Feux1.Vert = true;
                

                Fx.Feux2.Rouge = false;
                Fx.Feux2.Vert = true;
            }
            else if (Fx.Feux1.Vert == true)
            {  
                Fx.Feux1.Vert = false;
                Fx.Feux1.Orange = true;

                Fx.Feux2.Vert = false;
                Fx.Feux2.Orange = true;

                Fx.LectureEcritureReseau();
                Thread.Sleep(3000);

                Fx.Feux1.Rouge = true;
                Fx.Feux1.Orange = false;

                Fx.Feux2.Rouge = true;
                Fx.Feux2.Orange = false;
            }
            
            Fx.LectureEcritureReseau();
        }

        static void feux2(CmdFeux Fx)
        {
            Fx.LectureReseau();


            if (Fx.Feux3.Rouge == true)
            {
                Fx.Feux3.Rouge = false;
                Fx.Feux3.Vert = true;

                Fx.Feux4.Rouge = false;
                Fx.Feux4.Vert = true;
            }
            else if (Fx.Feux3.Vert == true)
            {
                Fx.Feux3.Vert = false;
                Fx.Feux3.Orange = true;

                Fx.Feux4.Vert = false;
                Fx.Feux4.Orange = true;
                Fx.LectureEcritureReseau();
                Thread.Sleep(3000);
                Fx.Feux3.Rouge = true;
                Fx.Feux3.Orange = false;

                Fx.Feux4.Rouge = true;
                Fx.Feux4.Orange = false;
            }

            Fx.LectureEcritureReseau();
        }
        
        static Boolean detection_pieton_1(CmdFeux Fx)
        {
            if (Fx.Feux1.BP == true || Fx.Feux2.BP == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static Boolean detection_voiture_1(CmdFeux Fx)
        {
            if (Fx.Feux3.Detecteur == false && Fx.Feux4.Detecteur == false && (Fx.Feux1.Detecteur == true || Fx.Feux2.Detecteur == true))
            {
                return true;
            }
            return false;
        }

        static Boolean detection_pieton_2(CmdFeux Fx)
        {
            if (Fx.Feux3.BP == true || Fx.Feux4.BP == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static Boolean detection_voiture_2(CmdFeux Fx)
        {
            if (Fx.Feux1.Detecteur == false && Fx.Feux2.Detecteur == false && (Fx.Feux3.Detecteur == true || Fx.Feux4.Detecteur == true))
            {
                return true;
            }
            return false;
        }

        static void setup(CmdFeux Fx)
        {
            Fx.Feux3.Rouge = true;
            Fx.Feux3.Vert = false;
            Fx.Feux3.Orange = false;

            Fx.Feux4.Rouge = true;
            Fx.Feux4.Vert = false;
            Fx.Feux4.Orange = false;

            Fx.Feux1.Rouge = false;
            Fx.Feux1.Vert = true;
            Fx.Feux1.Orange = false;

            Fx.Feux2.Rouge = false;
            Fx.Feux2.Vert = true;
            Fx.Feux2.Orange = false;

            Fx.LectureEcritureReseau();
        }

        static int[] LectureFichier()
        {
            string text = File.ReadAllText(@"..\..\..\Config.txt");
            String[] temps = text.Split('\n', ' ');
            int[] valeurs = new int[4];
             valeurs[0]= Convert.ToInt32(temps[2]) * 1000;
             valeurs[1]= Convert.ToInt32(temps[3]) * 1000;
             valeurs[2]= Convert.ToInt32(temps[4]) * 1000;
             valeurs[3] = Convert.ToInt32(temps[5]) * 1000;

             return valeurs;
        }

        static void EcritureFichier(String[] mesure)
        {
            for (int i = 0; i < mesure.Length; i++)
            {
                if (mesure[i] != "") {
                StreamWriter sw = File.AppendText(@"..\..\..\log.csv");
                sw.WriteLine(mesure[i]);
                sw.Close();
                mesure[i] = "";
                }
                
            }
        }

        static void releve(CmdFeux Fx, string[] mesure)
        {
            
            if (Fx.Feux1.BP == true && mesure[0]=="")
            {
                mesure[0] = DateTime.Now.ToString() + ";1;BP piéton";
            }
            if (Fx.Feux2.BP == true && mesure[1] == "")
            {
                mesure[1] = DateTime.Now.ToString() + ";2;BP piéton";
            }
            if (Fx.Feux3.BP == true && mesure[2] == "")
            {
                mesure[2] = DateTime.Now.ToString() + ";3;BP piéton";
            }
            if (Fx.Feux4.BP == true && mesure[3] == "")
            {
                mesure[3] = DateTime.Now.ToString() + ";4;BP piéton";
            }
            if (Fx.Feux1.Detecteur == true && mesure[4] == "")
            {
                mesure[4] = DateTime.Now.ToString() + ";1;Détection véhicule";
            }
            if (Fx.Feux2.Detecteur == true && mesure[5] == "")
            {
                mesure[5] = DateTime.Now.ToString() + ";2;Détection véhicule";
            }
            if (Fx.Feux3.Detecteur == true && mesure[6] == "")
            {
                mesure[6] = DateTime.Now.ToString() + ";3;Détection véhicule";
            }
            if (Fx.Feux4.Detecteur == true && mesure[7] == "")
            {
                mesure[7] = DateTime.Now.ToString() + ";4;Détection véhicule";
            }
            
        }

        static void Main(string[] args)
        {
            int[] tempsAttente = LectureFichier();
            string[] mesure = {"","","","","","","","" };
            double localDate;

            Boolean pieton=false;
            Boolean voiture=false;
    
            CmdFeux Fx = new CmdFeux(); // Création de l’objet permettant les opérations de lecture/ecriture
            Fx.IPCarrefour = "127.0.0.1";// Paramètrage de l’adresse IP de la maquette

            setup(Fx);

            do
            {
                Thread.Sleep(tempsAttente[0]);
                localDate = tempsAttente[0];
                do {
                    pieton = detection_pieton_1(Fx);
                    voiture = detection_voiture_1(Fx);
                    Thread.Sleep(100);
                    localDate += 100;
                    releve(Fx, mesure);
                } while (pieton == false && voiture == true && localDate < tempsAttente[1]);
                EcritureFichier(mesure);

                feux1(Fx);//rouge
                feux2(Fx);//vert

                Thread.Sleep(tempsAttente[2]);

                localDate = tempsAttente[2];
                do
                {
                    pieton = detection_pieton_2(Fx);
                    voiture = detection_voiture_2(Fx);
                    Thread.Sleep(100);
                    localDate += 100;
                    releve(Fx, mesure);
                } while (pieton == false && voiture == true && localDate < tempsAttente[3]);
                EcritureFichier(mesure);
                feux2(Fx);//rouge
                feux1(Fx);//vert
            } while (true);
        }
  
    }
}
