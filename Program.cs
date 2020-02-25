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
        /*****************************************************************\
         * Cycle Part 1                                                  *
         * BUT : Faire la première partie du cycle des feux              *
         * ENTREE : Variable de connexion à la maquette                  *
         *          Le temps minimum de vert pour la voie 1              *
         *          Le temps maximum de vert pour la voie 1              *
        \*****************************************************************/
        static void cyclePart1(CmdFeux Fx,int attenteMini, int attenteMaxi)
        {
            double localDate=0;//localDate : double, compteur (mesurer le temps)
            Boolean pieton=false;//pieton : variable booléen, si un pieton a appuyé sur un bouton
            Boolean voiture=false;//voiture: variable booléen, si on détecte une voiture
            string[] mesure = { "", "", "", "", "", "", "", "" };//mesure : tableau pour enregistrer les mesures des capteurs (detection véhicule/pieton)

                        //Ces instructions permettent que le feux reste au vert plus longtemps en respectant les différentes conditions
                        do
                        {
                            Fx.LectureEcritureReseau();//On fait une lecture réseau pour ne pas tomber en timeout et récuperer les infos sur les feux/voitures
                            if (!pieton)//si on détecte un pieton pas besoin de mesurer à nouveau (on pourrait perdre la valeur)
                            {
                                pieton = detection_pieton_1(Fx);//On appel la fonction pour savoir si des pietons ont appuyé sur un bouton
                            }
                            voiture = detection_voiture_1(Fx);//On appel la fonction pour savoir si on detecte des voitures
                            Thread.Sleep(100);//On fait une pause de 100ms dans le programme
                            localDate += 100;//On ajoute ce temps au compteur
                            releve(Fx, mesure);//On appel la fonction pour enregistrer les mesures que l'on a effectué
                        } while (localDate<attenteMini||(pieton == false && voiture == true && localDate < attenteMaxi));//On fait cela tant que l'on ne detecte pas de pieton, que l'on detecte des voitures et que le compteur n'a pas atteint la durée maximum de vert des feux de la voie 1 ou que l'on a pas dépassé le temps mini des feux de la voie 1
                        //fin
                    

                    EcritureFichier(mesure);//On appel la fonction pour enregistrer les mesures dans un fichier

                    feux1(Fx);//Appel de la fonction pour changer la couleur des feux sur la voie 1 (passage au rouge)
                    feux2(Fx);//Appel de la fonction pour changer la couleur des feux sur la voie 2 (passage au vert)
        }

        /*****************************************************************\
         * Cycle Part 2                                                  *
         * BUT : Faire la seconde partie du cycle des feux               *
         * ENTREE : Variable de connexion à la maquette                   *
         *          Le temps minimum de vert pour la voie 2              *
         *          Le temps maximum de vert pour la voie 2              *
        \*****************************************************************/
        static void cyclePart2(CmdFeux Fx, int attenteMini, int attenteMaxi)
        {
            double localDate = 0;//localDate : double, compteur (mesurer le temps)
            Boolean pieton = false;//pieton : variable booléen, si un pieton a appuyé sur un bouton
            Boolean voiture = false;//voiture: variable booléen, si on détecte une voiture
            string[] mesure = { "", "", "", "", "", "", "", "" };//On crée un tableau pour enregistrer les mesures des capteurs (detection véhicule/pieton)

            //Ces instructions permettent que le feux reste au vert plus longtemps en respectant les différentes conditions
            do
            {
                
                Fx.LectureEcritureReseau();//On fait une lecture réseau pour ne pas tomber en timeout et récuperer les infos sur les feux/voitures
                if (!pieton)//si on détecte un pieton pas besoin de mesurer à nouveau (on pourrait perdre la valeur)
                {
                    pieton = detection_pieton_2(Fx);//On appel la fonction pour savoir si des pietons ont appuyé sur un bouton
                }
                voiture = detection_voiture_2(Fx);//On appel la fonction pour savoir si on detecte des voitures
                Thread.Sleep(100);//On fait une pause de 100ms dans le programme
                localDate += 100;//On ajoute ce temps au compteur
                releve(Fx, mesure);//On appel la fonction pour enregistrer les mesures que l'on a effectué
            } while (localDate < attenteMini || (pieton == false && voiture == true && localDate <attenteMaxi));//On fait cela tant que l'on ne detecte pas de pieton, que l'on detecte des voitures et que le compteur na pas atteint la durée maximum de vert des feux de la voie 2 ou que l'on a pas dépassé le temps mini des feux de la voie 2
            //fin


            EcritureFichier(mesure);//On appel la fonction pour enregistrer les mesures dans un fichier

            feux2(Fx);//Appel de la fonction pour changer la couleur des feux sur la voie 2 (passage au rouge)
            feux1(Fx);//Appel de la fonction pour changer la couleur des feux sur la voie 1 (passage au vert)
        }

        /*****************************************************************\
         * Cycle feux 1                                                  *
         * BUT : Faire changer la couleur des feux de la voie 1          *
         * ENTREE : Variable de connexion à la maquette                  *
        \*****************************************************************/
        static void feux1(CmdFeux Fx)
        {
            Fx.LectureReseau();//On récupère les état des feux
            int localDate = 0;//localDate : double, compteur (mesurer le temps)

            if (Fx.Feux1.Rouge == true)//si le feux 1 est rouge
            {
                Fx.Feux1.Rouge = false;//on éteint le feux rouge 1
                Fx.Feux1.Vert = true;//On allume le feux vert 1


                Fx.Feux2.Rouge = false;//on éteint le feux rouge 2
                Fx.Feux2.Vert = true;//On allume le feux vert 2
            }
            else if (Fx.Feux1.Vert == true)//sinon si le feux est vert
            {
                Fx.Feux1.Vert = false;//on éteint le feux vert 1
                Fx.Feux1.Orange = true;//On allume le feux orange 1

                Fx.Feux2.Vert = false;//on éteint le feux vert 2
                Fx.Feux2.Orange = true;//On allume le feux orange 2

                Fx.LectureEcritureReseau();//On applique ces changement sur les feux

                //permet de ne pas tomber en timeout pendant le feux orange
                do
                {
                    Thread.Sleep(100);//On fait une pause de 100ms dans le programme
                    localDate += 100;//On ajoute ce temps au compteur
                    Fx.LectureEcritureReseau();//On fait une lecture réseau pour continuer à discuter avec la maquette
                } while (localDate < 3000);//On laisse le feux orange pendant 3s
                //fin

                Fx.Feux1.Rouge = true;//On allume le feux rouge 1
                Fx.Feux1.Orange = false;//on éteint le feux orange 1

                Fx.Feux2.Rouge = true;//On allume le feux rouge 1
                Fx.Feux2.Orange = false;//on éteint le feux orange 2
            }

            Fx.LectureEcritureReseau();//On applique ces changement sur les feux
        }

        /*****************************************************************\
         * Cycle feux 2                                                  *
         * BUT : Faire changer la couleur des feux de la voie 2          *
         * ENTREE : Variable de connexion à la maquette                  *
        \*****************************************************************/
        static void feux2(CmdFeux Fx)
        {
            Fx.LectureReseau();//On récupère les état des feux
            int localDate = 0;//localDate : double, compteur (mesurer le temps)

            if (Fx.Feux3.Rouge == true)//si le feux  3 est rouge
            {
                Fx.Feux3.Rouge = false;//on éteint le feux rouge 3
                Fx.Feux3.Vert = true;//On allume le feux vert 3

                Fx.Feux4.Rouge = false;//on éteint le feux rouge 4
                Fx.Feux4.Vert = true;//On allume le feux vert 4
            }
            else if (Fx.Feux3.Vert == true)
            {
                Fx.Feux3.Vert = false;//on éteint le feux vert 3
                Fx.Feux3.Orange = true;//On allume le feux orange 3

                Fx.Feux4.Vert = false;//on éteint le feux vert 4
                Fx.Feux4.Orange = true;//On allume le feux orange 4
                Fx.LectureEcritureReseau();//On applique ces changement sur les feux

                //permet de ne pas tomber en timeout pendant le feux orange
                do
                {
                    Thread.Sleep(100);//On fait une pause de 100ms dans le programme
                    localDate += 100;//On ajoute ce temps au compteur
                    Fx.LectureReseau();//On fait une lecture réseau pour continuer à discuter avec la maquette
                } while (localDate < 3000);//On laisse le feux orange pendant 3s
                //fin

                Fx.Feux3.Rouge = true;//On allume le feux rouge 3
                Fx.Feux3.Orange = false;//on éteint le feux orange 3

                Fx.Feux4.Rouge = true;//On allume le feux rouge 4
                Fx.Feux4.Orange = false;//on éteint le feux orange 4
            }

            Fx.LectureEcritureReseau();//On applique ces changement sur les feux
        }

        /*****************************************************************\
         * Detection pieton 1                                             *
         * BUT : Savoir si un pieton a appuyé sur un bouton de la voie 1  *
         * ENTREE : Variable de connexion à la maquette                   *
         * SORTIE : Variable booléenne de présence ou d'absence de pieton *
        \*****************************************************************/
        static Boolean detection_pieton_1(CmdFeux Fx)
        {
            if (Fx.Feux1.BP == true || Fx.Feux2.BP == true)//si un pieton a appuyé sur un bouton sur le feux 1 ou 2
            {
                return true;// on renvoie "vrais"
            }
            else
            {
                return false;//Sinon on renvoie "faux"
            }
        }

        /*****************************************************************\
         * Detection voiture 1                                            *
         * BUT : Savoir si une voiture est au rouge sur la voie 2         *
         * ENTREE : Variable de connexion à la maquette                   *
         * SORTIE : Variable booléenne de présence ou d'absence de voiture*
        \*****************************************************************/
        static Boolean detection_voiture_1(CmdFeux Fx)
        {
            if (Fx.Feux3.Detecteur == false && Fx.Feux4.Detecteur == false)//si on ne detecte pas de voiture sur la voie 2 (feux 3 et 4)
            {
                return true;// on renvoie "vrais"
            }
            return false;//Sinon on renvoie "faux"
        }

        /*****************************************************************\
         * Detection pieton 2                                             *
         * BUT : Savoir si un pieton a appuyé sur un bouton de la voie 2  *
         * ENTREE : Variable de connexion à la maquette                   *
         * SORTIE : Variable booléenne de présence ou d'absence de pieton *
        \*****************************************************************/
        static Boolean detection_pieton_2(CmdFeux Fx)
        {
            if (Fx.Feux3.BP == true || Fx.Feux4.BP == true)//si un pieton a appuyé sur un bouton sur le feux 3 ou 4
            {
                return true;// on renvoie "vrais"
            }
            else
            {
                return false;//Sinon on renvoie "faux"
            }
        }

        /*****************************************************************\
         * Detection voiture 2                                            *
         * BUT : Savoir si une voiture est au rouge sur la voie 1         *
         * ENTREE : Variable de connexion à la maquette                   *
         * SORTIE : Variable booléenne de présence ou d'absence de voiture*
        \*****************************************************************/
        static Boolean detection_voiture_2(CmdFeux Fx)
        {
            if (Fx.Feux1.Detecteur == false && Fx.Feux2.Detecteur == false)//si on ne detecte pas de voiture sur la voie 1 (feux 1 et 2)
            {
                return true;// on renvoie "vrais"
            }
            return false;//Sinon on renvoie "faux"
        }

        /*****************************************************************\
         * Setup                                                         *
         * BUT : Mettre en place l'état initiale des différents feux     *
         * ENTREE : Variable de connexion à la maquette                  *
        \*****************************************************************/
        static void setup(CmdFeux Fx)
        {
            //on met le feux 1 au vert et on désactive les autres couleurs 
            Fx.Feux1.Rouge = false;
            Fx.Feux1.Vert = true;
            Fx.Feux1.Orange = false;
            //

            //on met le feux 2 au vert et on désactive les autres couleurs
            Fx.Feux2.Rouge = false;
            Fx.Feux2.Vert = true;
            Fx.Feux2.Orange = false;
            //

            //on met le feux 3 au rouge et on désactive les autres couleurs
            Fx.Feux3.Rouge = true;
            Fx.Feux3.Vert = false;
            Fx.Feux3.Orange = false;
            //

            //on met le feux 4 au rouge et on désactive les autres couleurs
            Fx.Feux4.Rouge = true;
            Fx.Feux4.Vert = false;
            Fx.Feux4.Orange = false;
            //            

            Fx.LectureEcritureReseau();//On applique les états attribués au dessus
        }

        /*****************************************************************\
         * Choix IP                                                      *
         * BUT : Choisir l'ip de connection (la simulation ou maquette)  *
         * ENTREE : Variable de connexion à la maquette                  *
        \*****************************************************************/
        static void choixIP(CmdFeux Fx)
        {
            string choixIP;//choixIP, string, enregistrer l'ip de connexion

            do
            {
                Console.WriteLine("Voulez vous vous connecter à la maquette[0] ou à la simulation[1]");//affiche du texte dans la console pour demander où
                choixIP = Convert.ToString(Console.ReadLine());//on récupére le choix de l'utilisateur

            } while (choixIP != "0" && choixIP != "1");//on redemande tant que l'utilisateur se trombe de réponse
            if (choixIP == "0")//si n choisit la maquete
            {
                Fx.IPCarrefour = "172.20.54.111";// Paramètrage de l’adresse IP de la maquette
            }
            else if (choixIP == "1")//si on choisit la simulation
            {
                Fx.IPCarrefour = "127.0.0.1";// Paramètrage de l’adresse IP de la simulation
            }
            Console.Clear();//on efface la console

            if (Fx.LectureEcritureReseau() == true)//On regarde si on est bien connecté
            {
                Console.WriteLine("Connecté !\nAppuyer sur 'Echap' pour quitter");//afficher qu'on s'est bien connecté et la commmande pour quitter
            }
            else//si on est pas connecté
            {
                Console.WriteLine("Erreur de connexion !\nAppuyer sur 'Echap' pour quitter\nPuis relancer le programme");//afficher qu'on ne s'est pas connecté
            }
            
            
            
        }

        /*****************************************************************\
         * Relevé                                                         *
         * BUT : Relevé les capteurs (detection de voitures/ pietons)     *
         * ENTREE : Variable de connexion à la maquette                   *
         *          Tableau contenant les mesures effectuées précédemment *
         * SORTIE : Tableau contenant les mesures effectuées              *
        \*****************************************************************/
        static void releve(CmdFeux Fx, string[] mesure)
        {

            if (Fx.Feux1.BP == true && mesure[0] == "")// si un pieton appuyé sur le bouton du feux 1 et que l'on a pas déja enregistré cette information
            {
                mesure[0] = DateTime.Now.ToString() + ";1;BP piéton";// on enregistre dans le tableau la date, la voie et que c'est un bouton de piéton
            }
            if (Fx.Feux2.BP == true && mesure[1] == "")// si un pieton appuyé sur le bouton du feux 2 et que l'on a pas déja enregistré cette information
            {
                mesure[1] = DateTime.Now.ToString() + ";2;BP piéton";// on enregistre dans le tableau la date, la voie et que c'est un bouton de piéton
            }
            if (Fx.Feux3.BP == true && mesure[2] == "")// si un pieton appuyé sur le bouton du feux 3 et que l'on a pas déja enregistré cette information
            {
                mesure[2] = DateTime.Now.ToString() + ";3;BP piéton";// on enregistre dans le tableau la date, la voie et que c'est un bouton de piéton
            }
            if (Fx.Feux4.BP == true && mesure[3] == "")// si un pieton appuyé sur le bouton du feux 3 et que l'on a pas déja enregistré cette information
            {
                mesure[3] = DateTime.Now.ToString() + ";4;BP piéton";// on enregistre dans le tableau la date, la voie et que c'est un bouton de piéton
            }
            if (Fx.Feux1.Detecteur == true && mesure[4] == "")// si une voiture est au feux 1 et que l'on a pas déja enregistré cette information
            {
                mesure[4] = DateTime.Now.ToString() + ";1;Détection véhicule";// on enregistre dans le tableau la date, la voie et que c'est une voiture
            }
            if (Fx.Feux2.Detecteur == true && mesure[5] == "")// si une voiture est au feux 2 et que l'on a pas déja enregistré cette information
            {
                mesure[5] = DateTime.Now.ToString() + ";2;Détection véhicule";// on enregistre dans le tableau la date, la voie et que c'est une voiture
            }
            if (Fx.Feux3.Detecteur == true && mesure[6] == "")// si une voiture est au feux 3 et que l'on a pas déja enregistré cette information
            {
                mesure[6] = DateTime.Now.ToString() + ";3;Détection véhicule";// on enregistre dans le tableau la date, la voie et que c'est une voiture
            }
            if (Fx.Feux4.Detecteur == true && mesure[7] == "")// si une voiture est au feux 4 et que l'on a pas déja enregistré cette information
            {
                mesure[7] = DateTime.Now.ToString() + ";4;Détection véhicule";// on enregistre dans le tableau la date, la voie et que c'est une voiture
            }

        }

        /*****************************************************************\
         * Lecture fichier                                               *
         * BUT : Récupérer la configuration des feux ( la durée du vert) *
         * SORTIE : Tableau d'entier de durée des différents feux        *
        \*****************************************************************/
        static int[] LectureFichier()
        {
            string text = File.ReadAllText(@"..\..\..\Config.txt"); //text, string, récupère le texte contenu dans le fichier Config.txt
            String[] temps = text.Split('\n', ' ');//temps, tableau, on divise ce texte par colone ('espace') et pas ligne ('\n') text
            int[] valeurs = new int[4];// valeurs, tableau d'entiers, création du tableau de taille 4
             valeurs[0]= Convert.ToInt32(temps[2]) * 1000;//la troisième valeur du tableau correspond au temps des feux vert de la voie 1 minimum ( on le multiplie par 1000 pour l'avoir en milliseconde)
             valeurs[1] = Convert.ToInt32(temps[3]) * 1000;//la quatrième valeur du tableau correspond au temps des feux vert de la voie 1 maximum ( on le multiplie par 1000 pour l'avoir en milliseconde)
             valeurs[2] = Convert.ToInt32(temps[4]) * 1000;//la cinquième valeur du tableau correspond au temps des feux vert de la voie 2 minimum ( on le multiplie par 1000 pour l'avoir en milliseconde)
             valeurs[3] = Convert.ToInt32(temps[5]) * 1000;//la sixième valeur du tableau correspond au temps des feux vert de la voie 2 maximum ( on le multiplie par 1000 pour l'avoir en milliseconde)

             return valeurs;//On renvoie le tableau contenant tous les temps pour les feux en milliseconde
        }

        /*****************************************************************\
         * Ecriture fichier                                              *
         * BUT : Savoir si une voiture est au rouge sur la voie 2        *
         * ENTREE : Variable de connexion à la maquette                  *
         *          Tableau contenant les mesures effectuées                                            *
        \*****************************************************************/
        static void EcritureFichier(String[] mesure)
        {
            for (int i = 0; i < mesure.Length; i++)//Boucle pour avoir accés a toutes les valeurs du tableau "mesure"
            {
                if (mesure[i] != "") {//Si la valeur n'est pas vide
                StreamWriter sw = File.AppendText(@"..\..\..\log.csv");//sw, pour ecrire dans le fichier, on ouvre le fichier .csv
                sw.WriteLine(mesure[i]);//on ajoute la valeur contenu dans le tableau
                sw.Close();//on ferme le fichier .csv
                mesure[i] = "";//on vide le contenu du tableau
                }
                
            }
        }


        /*****************************************************************\
         * Main                                                          *
         * BUT : Appeler les différentes fonctions (mise en place        *
         *          et boucle) et permettre une sortie de cette boucle   *
        \*****************************************************************/
        static void Main(string[] args)
        {
            int[] tempsAttente = LectureFichier();//On appel la fonction pour recupérer dans un tableau les durées maximum et minimum des feux.
  
            CmdFeux Fx = new CmdFeux(); // Création de l’objet permettant les opérations de lecture/ecriture et de contrôle du feux
            
            choixIP(Fx);//appel de la fonction pour choisir l'IP
            setup(Fx);//appel de la fonction pour initialiser les feux

            ConsoleKeyInfo tch = new ConsoleKeyInfo();//Initialisation d'une variable pour connaitre les touches qui sont appuyé

            while(true)//boucle infini
            {
                if(!Console.KeyAvailable){//si pas de touche enfoncée

                    cyclePart1(Fx, tempsAttente[0], tempsAttente[1]);//Appel de la fonction pour la première partie du cycle

                    cyclePart2(Fx, tempsAttente[2], tempsAttente[3]);//Appel de la fonction pour la seconde partie du cycle
                    

                }else{//sinon si on a une touche appuyé
                    tch = Console.ReadKey();//tch, on récupère la touche appuyé
                    if (tch.Key == ConsoleKey.Escape)//vraie si la touche appuyé est echap
                    {
                        break;//on sort de la boucle infini
                    }
                }
            }
        }
  
    }
}
