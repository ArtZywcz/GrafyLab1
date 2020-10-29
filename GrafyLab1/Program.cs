
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.IO;

namespace GrafyLab1
{
    class Program{
        static List<Graph> mojParser() {

            string[] data2 = File.ReadAllLines("C:\\Users\\Artur\\source\\repos\\GrafyLab1\\GrafyLab1\\sredni.dot"); //Pobierz plik
            Array.Resize(ref data2, data2.Length - 1); //Usuń ostatni wiersz, jest to tylko '}'
            data2 = data2.Skip(1).ToArray(); //Usuń pierwszy wiersz
            for (int i1 = 0; i1 < data2.Length; i1++) { //Usuń każdy średnik
                data2[i1] = data2[i1].Remove(data2[i1].Length - 1);
            }

            List<Graph> test = new List<Graph>();

            foreach (string line in data2) {
                if (!line.Contains(' ')) test.Add(new Graph { id = int.Parse(line), connectsTo = new List<int>(new int[] {}) } ); //Jeżeli nie ma w wierzu spacji to znaczy że jest to id wierzchołka, stwórz nowy wierzchołek i nadaj mu to id
                else { //Jeżeli ma spacje jest to połączenie, dodaj do jednego i drugiego id połączenie
                    char[] charArr = line.ToCharArray();
                    string number1 = "";
                    string number2 = "";
                    bool foundSpace = false;

                    for (int j = 0; j<charArr.Length; j++) {

                        if (charArr[j] == ' ') { 
                            foundSpace = true;
                            j += 4;
                        }
                        if (!foundSpace) number1 += charArr[j];
                        else number2 += charArr[j];
                    }

                    //test[test.FindIndex(x => x.id == int.Parse(number1))].connectsTo.Add(int.Parse(number2));//Powinno być tak jeśli id może być różne a nie po kolei, ale jest o wiele wiele dłużej
                    //test[test.FindIndex(x => x.id == int.Parse(number2))].connectsTo.Add(int.Parse(number1));

                    test[int.Parse(number1)].connectsTo.Add(int.Parse(number2));
                    test[int.Parse(number2)].connectsTo.Add(int.Parse(number1));
                }
            }

            return test;
        }
        static void Main(string[] args) {

            

            Graph tescik = new Graph();
            List<Graph> test = new List<Graph>();
            /*
            //TESTOWY GRAF Z ZADANIA TODO POBIERANIE Z PLIKU
            

            test.Add( new Graph { id = 0, connectsTo = new List<int>(new int[] { 4, 2 }) });
            test.Add(new Graph { id = 1, connectsTo = new List<int>(new int[] { 2 }) });
            test.Add(new Graph { id = 2, connectsTo = new List<int>(new int[] { 1, 0, 3 }) });
            test.Add(new Graph { id = 3, connectsTo = new List<int>(new int[] { 2 }) });
            test.Add(new Graph { id = 4, connectsTo = new List<int>(new int[] { 6, 0, 8 }) });
            test.Add(new Graph { id = 5, connectsTo = new List<int>(new int[] { 6, 8 }) });
            test.Add(new Graph { id = 6, connectsTo = new List<int>(new int[] { 5, 4 }) });
            test.Add(new Graph { id = 7, connectsTo = new List<int>(new int[] {  }) });
            test.Add(new Graph { id = 8, connectsTo = new List<int>(new int[] { 4, 5 }) });
            */

            test = mojParser();

            string data = JsonConvert.SerializeObject(test);//Tworzenie kopii danych żeby je dodać potem do tablicy
            var copy = JsonConvert.DeserializeObject<List<Graph>>(data);



            int n = test.Count; //Przykładowy rozmiar, w wersji końcowej ilość wierzchołków
            List<List<Graph>> graphs = new List<List<Graph>>();


            //graphs.Add(test);
            graphs.Add(copy);
            bool gotGroup = false;
            bool dontDelete = false;

            for (int i = 0, j = 1, graphNow = 0 ; i<graphs[0].Count -1;) { //i to wierzchołek który aktualnie sprawdzamy w pierwszej grupie, j to wierzchołek z którym porównujemy
                
                
                
                if (graphs[0][i].connectsTo.Contains(graphs[graphNow][j].id)) { //Jeśli dany wierzchołek 'i' ma połączenie z wierzchołkiem 'j' idź do następnej grupy
                    graphNow++;
                    j = 0;

                }
                else {


                    if (j == graphs[graphNow].Count -1) { //Jeśli nie ma już więcej elementów w tej grupie
                        if (graphNow != 0) graphs[graphNow].Add(graphs[0][i]);//Jeśli nie jest to grupa 0 to dodaj go tutaj
                        else dontDelete = true;//Jeśli jest to zerowa to nie usuwaj go, zamiast tego przesuń potem 'i' i 'j'

                        gotGroup = true;
                        j = 0;          
                        graphNow++;

                        
                        
                    }
                    else j++; //jeśli nie ma połączenia to sprawdź następny wierzchołek



                }
                
                if (graphNow == graphs.Count && j == 0) {//Jeżeli już nie ma więcej grup i jest to ostatni element w tej grupie
                    if (gotGroup == false) { //Jeli nie trafi do żadnej grupy to stwórz mu nową i tam go dodaj
                        graphs.Add(new List<Graph>()); //Utwórz nową listę
                        graphs[graphNow].Add(graphs[0][i]); //Dodaj ten wierzchołek do niej
                    }

                    if (dontDelete) i++; //Jeśli ma zostać w zerowej to zwiększamy i o 1
                    else graphs[0].RemoveAt(i);

                    j = i + 1; //RESET WARTOŚCI
                    graphNow = 0;
                    gotGroup = false;
                    dontDelete = false;
                }
                
               
            }

            //SPRAWDZANIE DRUGIEGO WARUNKU


            
            bool anyGraph = false;

            for (int i = 0; i<graphs.Count; i++) {//wybierz grupę wierzchołków

                bool goodGraph = true;
                for (int j = 0; j < test.Count; j++) { //Sprawdzaj wszystkie wierzchołki

                    
                    if ((graphs[i].FindIndex(x => x.id == test[j].id)) == -1) {   //Znajdź wierzchołek którego nie ma w tym zbiorze

                        
                        bool noConnection = true;
                        foreach (int value in test[j].connectsTo) {

                            if (graphs[i].FindIndex(x => x.id == value) != -1) noConnection = false; //Jeśli jest połączenie między tymi dwoma to jedziemy dalej, jeśli nie ma to wywala graf
                        }

                        if (noConnection) { 
                            j = test.Count;
                            goodGraph = false;
                            anyGraph = true;
                        } //Jeśli jest połączenie to przejdź do następnego wierzchołka


                    }
                    
                }
                if (goodGraph) {
                    Console.WriteLine("Zbiór nr:" + i + " JEST ROZWIĄZANIEM TEGO ZADANIA!!!\n Czyli wierzchołki: ");
                    foreach (Graph x in graphs[i]) Console.Write(x.id + ", ");

                }

                

            }
            if (!anyGraph) Console.WriteLine("Ten graf nie posiada takiego zbioru");
        }
    }
}
