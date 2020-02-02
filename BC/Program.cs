using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BC
{
    class Program
    {
        static List<Node> nodes = new List<Node>();
        static List<Rules> rules = new List<Rules>();
        static List<Rules> rulesUsed = new List<Rules>();
        static List<Node> startNodesL = new List<Node>();
        static List<Node> nodeBank = new List<Node>();
        static Node earlyInfered;
        static Node goalNode;
        static int trePlus = 0;
        static bool endProg = false;
        static bool goal = false;
        static int nodesIhad = 0;
        static int iteration = 1;
        static string[] eachLine;
        static StreamWriter file ;

        static void Main(string[] args)
        {
            Part1();
            file.Close();
        }
        static public Rules PickRule(Node endNode, List<Rules> usedOnes, string tre)
        {
            int x = 0;
            foreach (var item in rules)
            {
                x++;
                if (item.getAim() == endNode && !usedOnes.Contains(item))
                {
                    if (item.getAim().calledH() > 1)
                    {
                        file.WriteLine("   " + iteration + ") " + tre + "Goal " + item.getAim().getName() + ". Cycle. Back, FAIL.");
                        iteration++;
                        item.getAim().calledM();
                        return null;
                    }

                    else
                        return item;
                }
                else
                {
                    if (x == rules.Count)
                    {
                        if (usedOnes.Count > 0)
                        {
                            if (endNode == goalNode)
                                endProg = true;
                            file.WriteLine("   " + iteration + ") " + tre + "Goal " + endNode.getName() + ". No more rules . Back, FAIL.");
                        }
                        else
                        {
                            if (endNode == goalNode)
                                endProg = true;

                            file.WriteLine("   " + iteration + ") " + tre + "Goal " + endNode.getName() + ". No rules . Back, FAIL.");
                        }
                        iteration++;
                    }
                }
            }
            return null;
        }

        static void Procedure_Search(Char endChar, int lastPoint)
        {
            goalNode = nodes[lastPoint];
            while (!nodeBank.Contains(nodes[lastPoint]))
            {
                List<Rules> usedOnes = new List<Rules>();
                START0:
                Rules rulesUsing = PickRule(nodes[lastPoint], usedOnes, "");
                usedOnes.Add(rulesUsing);
                if (endProg)
                    goto Endofit;
                file.Write("   " + iteration + ") Goal " + nodes[lastPoint].getName() + ". Find R" + rulesUsing.getRuleNo() + ":");
                iteration++;

                if (rulesUsing.getRulElem() == 3)
                {
                    rulesUsing.getNode().calledP();
                    file.WriteLine(rulesUsing.getNode().getName() + "," + rulesUsing.getNode2().getName() + "," + rulesUsing.getNode3().getName() +
                    "->" + rulesUsing.getAim().getName() + ". New goals " +
                    rulesUsing.getNode().getName() + ", " + rulesUsing.getNode2().getName() + ", " + rulesUsing.getNode3().getName() + ".");
                    if (Procedure_Search2(rulesUsing.getNode(), 1))
                    {
                        nodeBank.Add(rulesUsing.getNode());
                        rulesUsing.getNode2().calledP();
                        iteration++;
                        if (Procedure_Search2(rulesUsing.getNode2(), 2))
                        {
                            nodeBank.Add(rulesUsing.getNode2());
                            rulesUsing.getNode3().calledP();
                            iteration++;
                            if (Procedure_Search2(rulesUsing.getNode3(), 3))
                            {
                                nodeBank.Add(rulesUsing.getNode3());
                                trePlus = 0;
                                initialState(rulesUsing.getNode3(), "-");
                                file.WriteLine(". Back, OK.");

                                iteration++;
                                rulesUsed.Add(rulesUsing);
                                goal = true;
                            }
                            else
                                nodeBank.Remove(rulesUsing.getNode2());

                        }
                        else
                            nodeBank.Remove(rulesUsing.getNode());
                    }
                }
                else if (rulesUsing.getRulElem() == 2)
                {
                    rulesUsing.getNode().calledP();
                    file.WriteLine(rulesUsing.getNode().getName() + "," + rulesUsing.getNode2().getName() +
                   "->" + rulesUsing.getAim().getName() + ". New goals " +
                       rulesUsing.getNode().getName() + "," + rulesUsing.getNode2().getName() + ".");
                    if (Procedure_Search2(rulesUsing.getNode(), 1))
                    {

                        nodeBank.Add(rulesUsing.getNode());
                        initialState(rulesUsing.getNode(), "-");
                        file.WriteLine(". Back, OK.");
                        rulesUsing.getNode2().calledP();
                        nodeZero();
                        iteration++;
                        if (Procedure_Search2(rulesUsing.getNode2(), 1))
                        {
                            if (!nodeBank.Contains(rulesUsing.getNode2()))
                                nodeBank.Add(rulesUsing.getNode2());
                            trePlus = 0;
                            initialState(rulesUsing.getNode2(), "-");
                            file.WriteLine(". Back, OK.");

                            iteration++;
                            rulesUsed.Add(rulesUsing);
                            goal = true;
                        }
                        else
                            nodeBank.Remove(rulesUsing.getNode());

                    }
                }
                else
                {
                    rulesUsing.getNode().calledP();
                    file.WriteLine(rulesUsing.getNode().getName() +
                          "->" + rulesUsing.getAim().getName() + ". New goal " +
                          rulesUsing.getNode().getName() + ".");
                    if (Procedure_Search2(rulesUsing.getNode(), 1))
                    {
                        if (!nodeBank.Contains(rulesUsing.getNode()))
                            nodeBank.Add(rulesUsing.getNode());
                        trePlus = 0;
                        initialState(rulesUsing.getNode(), "-");
                        file.WriteLine(". Back, OK.");

                        iteration++;
                        rulesUsed.Add(rulesUsing);
                        goal = true;
                    }

                }

                if (goal)
                {
                    nodeBank.Add(nodes[lastPoint]);
                    initialState(nodes[lastPoint], "");
                    file.WriteLine(". OK.");
                }
                if (rulesUsing != null && !goal)
                {
                    rulesUsed.Clear();
                    goto START0;

                }
                Endofit:
                break;
            }
        }
        static bool Procedure_Search2(Node goalNode, int step)
        {
            earlyInfered = goalNode;
            string tre = new string('-', step);
            List<Rules> usedOnes = new List<Rules>();
            START1:
            if (nodeBank.Contains(goalNode))
            {
                return true;
            }
            Rules rulesUsing = PickRule(goalNode, usedOnes, tre);
            usedOnes.Add(rulesUsing);
            if (rulesUsing != null)
            {

                file.Write("   " + iteration + ") " + tre + "Goal " + goalNode.getName() + ". Find R" + rulesUsing.getRuleNo() + ":");
                iteration++;


                if (rulesUsing.getRulElem() == 3)
                {
                    rulesUsing.getNode().calledP();

                    file.WriteLine(rulesUsing.getNode().getName() + "," + rulesUsing.getNode2().getName() + "," + rulesUsing.getNode3().getName() +
                        "->" + rulesUsing.getAim().getName() + ". New goals " +
                        rulesUsing.getNode().getName() + ", " + rulesUsing.getNode2().getName() + ", " + rulesUsing.getNode3().getName() + ".");
                    if (Procedure_Search2(rulesUsing.getNode(), step + 1))
                    {
                        if (!nodeBank.Contains(rulesUsing.getNode()))
                            nodeBank.Add(rulesUsing.getNode());
                        rulesUsing.getNode2().calledP();

                        if (Procedure_Search2(rulesUsing.getNode2(), step + 2))
                        {
                            if (!nodeBank.Contains(rulesUsing.getNode2()))
                                nodeBank.Add(rulesUsing.getNode2());

                            rulesUsing.getNode3().calledP();
                            if (Procedure_Search2(rulesUsing.getNode3(), step + 3))
                            {
                                if (earlyInfered == rulesUsing.getNode3() && trePlus < 1)
                                    trePlus++;
                                if (!nodeBank.Contains(rulesUsing.getNode3()))
                                    nodeBank.Add(rulesUsing.getNode3());
                                initialState(rulesUsing.getNode3(), tre);
                                iteration++;
                                rulesUsed.Add(rulesUsing);
                                file.WriteLine(". Back, OK.");
                                return true;
                            }

                        }
                        else
                            nodeBank.Remove(rulesUsing.getNode2());
                    }
                    nodeBank.Remove(rulesUsing.getNode());

                }
                else if (rulesUsing.getRulElem() == 2)
                {
                    rulesUsing.getNode().calledP();

                    file.WriteLine(rulesUsing.getNode().getName() + "," + rulesUsing.getNode2().getName() +
                       "->" + rulesUsing.getAim().getName() + ". New goals " +
                           rulesUsing.getNode().getName() + "," + rulesUsing.getNode2().getName() + ".");
                    if (Procedure_Search2(rulesUsing.getNode(), step + 1))
                    {
                        if (!nodeBank.Contains(rulesUsing.getNode()))
                            nodeBank.Add(rulesUsing.getNode());
                        rulesUsing.getNode2().calledP();
                        if (Procedure_Search2(rulesUsing.getNode2(), step + 2))
                        {
                            if (earlyInfered == rulesUsing.getNode2() && trePlus < 1)
                                trePlus++;
                            if (!nodeBank.Contains(rulesUsing.getNode2()))
                                nodeBank.Add(rulesUsing.getNode2());
                            initialState(rulesUsing.getNode2(), tre);
                            iteration++;
                            file.WriteLine(". Back, OK.");
                            rulesUsed.Add(rulesUsing);
                            return true;
                        }
                    }
                    else
                        nodeBank.Remove(rulesUsing.getNode());
                }
                else
                {
                    rulesUsing.getNode().calledP();
                    file.WriteLine(rulesUsing.getNode().getName() +
                          "->" + rulesUsing.getAim().getName() + ". New goal " +
                          rulesUsing.getNode().getName() + ".");
                    if (Procedure_Search2(rulesUsing.getNode(), step + 1))
                    {
                        if (earlyInfered == rulesUsing.getNode() && trePlus < 1)
                            trePlus++;
                        if (!nodeBank.Contains(rulesUsing.getNode()))
                            nodeBank.Add(rulesUsing.getNode());
                        initialState(rulesUsing.getNode(), tre);
                        iteration++;
                        file.WriteLine(". Back, OK.");
                        rulesUsed.Add(rulesUsing);
                        return true;
                    }
                }
            }

            if (rulesUsing == null)
                return false;
            else
                goto START1;

        }
        static void nodeZero()
        {
            foreach (var item in nodes)
            {
                if (!nodeBank.Contains(item) && item.calledH() > 0)
                {
                    item.calledM();
                }
            }

        }

        static public void Part1()
        {

            Console.Write("Enter Test No : ");
            int tests = int.Parse(Console.ReadLine());
            
            //file.Write("This is how it is written");
            file = new StreamWriter(@"C:\Users\ASUS\Desktop\BC\BC\bin\Debug\out" + tests + ".txt");
            //Test Seçme
            
            int x = 0, e = 0; char endNodes = ' ';
            eachLine = File.ReadAllLines("Rules -" + tests + ".txt");
            file.WriteLine("Semih Yoltan. Riga Technical University, Mathematic and Informatics");
            file.WriteLine("Test Number: "+tests);
            
            file.WriteLine("PART 1. Data\n");
            createNodes();
            endNodes = createRules();
           
            file.WriteLine();
            file.WriteLine("   3) Goal");
            file.WriteLine("\t   " + endNodes);
            foreach (var item in nodes)
            {
                if (item.getName() == endNodes)
                { x = e; }
                else
                    e++;
            }
            if (nodeBank.Contains(nodes[x]))
            {
                file.WriteLine("PART 2. Trace\n");
                file.WriteLine("PART 3. Results");
                file.WriteLine(" Goal " + nodes[x].getName() + " in facts. Empty path.");
            }
            else
                Part2(x, endNodes);
        }
        static public void Part2(int x, char endNodes)
        {
            file.WriteLine("PART 2. Trace");
            Procedure_Search(endNodes, x);
            file.WriteLine();
            file.WriteLine("PART 3. Results");
            if (goal)
            {
                file.WriteLine("  1) Goal " + endNodes + " achieved");
                file.Write("  2) Path: ");
            }
            else
                file.Write("  1) No Path");
            usedRules(); file.WriteLine();
            file.Close();
        }

        static public void createNodes()
        {
            nodes.Add(new Node('A'));//0
            nodes.Add(new Node('B'));//1
            nodes.Add(new Node('C'));//2
            nodes.Add(new Node('D'));//3
            nodes.Add(new Node('E'));//4
            nodes.Add(new Node('F'));//4
            nodes.Add(new Node('G'));//5
            nodes.Add(new Node('H'));//7
            nodes.Add(new Node('I'));//8
            nodes.Add(new Node('J'));//9
            nodes.Add(new Node('K'));//10
            nodes.Add(new Node('L'));//11
            nodes.Add(new Node('M'));//12
            nodes.Add(new Node('N'));//13
            nodes.Add(new Node('O'));//14
            nodes.Add(new Node('P'));//15
            nodes.Add(new Node('R'));//16
            nodes.Add(new Node('S'));//17
            nodes.Add(new Node('T'));//18
            nodes.Add(new Node('U'));//19
            nodes.Add(new Node('V'));//20
            nodes.Add(new Node('X'));//21
            nodes.Add(new Node('Y'));//22
            nodes.Add(new Node('Z'));//23
        }
        static public char createRules()
        {
            int ruleNo = 1;
            bool takeRule = false;
            bool takeFacts = false;
            bool quit = false;
            string splittedItem;
            foreach (var item in eachLine)
            {
                if (takeFacts)
                {
                    file.WriteLine("   1) Rules");
                    printRules(); file.WriteLine();
                    file.WriteLine("   2) Facts"); startNodes(item); takeFacts = false;
                }
                if (quit) return item[0];
                if (item == "3) Goal") quit = true;
                if (item == "2) Facts") takeFacts = true;

                if (item.Length < 2) takeRule = false;

                if (item.Contains('\t'))
                    splittedItem = item.Split('\t')[0];
                else
                    splittedItem = item;
                if (takeRule)
                {
                    if (splittedItem.Length < 4)
                    {
                        foreach (var item2 in nodes)
                        {
                            foreach (var item3 in nodes)
                            {
                                if (item2.getName() == splittedItem.Split(' ')[0][0] && item3.getName() == splittedItem.Split(' ')[1][0])
                                {
                                    rules.Add(new Rules(item3, item2, ruleNo));
                                    ruleNo++;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (splittedItem.Length > 5)
                        {
                            foreach (var item2 in nodes)
                            {
                                foreach (var item3 in nodes)
                                {
                                    foreach (var item4 in nodes)
                                    {
                                        foreach (var item5 in nodes)
                                        {
                                            if (item2.getName() == splittedItem.Split(' ')[0][0] && item3.getName() == splittedItem.Split(' ')[1][0]
                                                && item4.getName() == splittedItem.Split(' ')[2][0] && item5.getName() == splittedItem.Split(' ')[3][0])
                                            {
                                                rules.Add(new Rules(item3, item4, item5, item2, ruleNo));
                                                ruleNo++;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        else
                            foreach (var item2 in nodes)
                            {
                                foreach (var item3 in nodes)
                                {
                                    foreach (var item4 in nodes)
                                    {
                                        if (item2.getName() == splittedItem.Split(' ')[0][0] && item3.getName() == splittedItem.Split(' ')[1][0]
                                            && item4.getName() == splittedItem.Split(' ')[2][0])
                                        {
                                            rules.Add(new Rules(item3, item4, item2, ruleNo));
                                            ruleNo++;
                                        }
                                    }
                                }

                            }
                    }
                }
                if (item == "1) Rules") takeRule = true;

            }
            return ' ';
        }
        static public void startNodes(string x)
        {
            file.Write("\t   ");
            file.Write(x);
            int nodestep = 0;
            foreach (var item in x.Split(' '))
            {
                foreach (var item2 in nodes)
                {
                    if (item[0] != ' ')
                    {
                        if (item2.getName() == item[0])
                        {
                            startNodesL.Add(item2);
                            nodeBank.Add(item2);
                            nodestep++;
                        }
                    }
                    else
                    {
                        if (item2.getName() == item[1])
                        {
                            startNodesL.Add(item2);
                            nodeBank.Add(item2);
                            nodestep++;
                        }
                    }

                }
              
            }

            nodesIhad = nodeBank.Count;
        }
        static public void initialState(Node nodee, String tre)
        {
            var index = 1;
            tre = new string('-', (tre.Length + trePlus));

            if (startNodesL.Contains(nodee))
                file.Write("   "+iteration + ") " + tre + "Goal " + nodee.getName() + ". Fact(initial), " +
                       "as facts ");
            else if (nodee == earlyInfered)
                file.Write("   " + iteration + ") " + tre + "Goal " + nodee.getName() + ". Fact(early inferred), " +
                   "as facts ");
            else
                file.Write("   " + iteration + ") " + tre + "Goal " + nodee.getName() + ". Fact(presently inferred)." +
                   "Facts ");
            if (!startNodesL.Contains(nodee))
                foreach (var item in nodeBank)
                {
                    file.Write(item.getName());
                    if (index < nodeBank.Count) file.Write(", ");
                    index++;
                    if (index == nodesIhad + 1) file.Write(" and ");
                }
            else
                foreach (var item in startNodesL)
                {
                    file.Write(item.getName());
                    if (index < nodeBank.Count) file.Write(", ");
                }
        }
        static public void printNodes()
        {
            var index = 1;
            foreach (var item in nodes)
            {
                file.Write(item.getName());
                if (index < nodes.Count) file.Write(", ");
                index++;
            }
        }
        static public void usedRules()
        {
            var index = 1;
            foreach (var item in rulesUsed)
            {
                file.Write("R" + item.getRuleNo());
                if (index < rulesUsed.Count) file.Write(", ");
                index++;
            }
            file.Write(".");
        }
        static public void printRules()
        {
            var index = 0;
            foreach (var item in rules)
            {
                if (item.getRulElem() == 1)
                {
                    file.Write("\tR" + item.getRuleNo() + ": " + item.getNode().getName());
                    if (index < rules.Count) file.Write(" -> ");
                    file.WriteLine(item.getAim().getName());
                    index++;
                }
                else
                {
                    if (item.getRulElem() == 3)
                    {
                        file.Write("\tR" + item.getRuleNo() + ": " + item.getNode().getName() + ", " + item.getNode2().getName()
                            + ", " + item.getNode3().getName());
                        if (index < rules.Count) file.Write(" -> ");
                        file.WriteLine(item.getAim().getName());
                        index++;
                    }
                    else
                    {
                        file.Write("\tR" + item.getRuleNo() + ": " + item.getNode().getName() + ", " + item.getNode2().getName());
                        if (index < rules.Count) file.Write(" -> ");
                        file.WriteLine(item.getAim().getName());
                        index++;
                    }
                }
            }
        }
    }
    class Node
    {
        char name;
        int called = 0;
        public Node(char name)
        {
            this.name = name;
        }
        public char getName()
        {
            return name;
        }
        public void calledP()
        {
            called++;
        }
        public void calledM()
        {
            called--;
        }
        public int calledH()
        {
            return called;
        }

    }
    class Rules
    {
        int RuleNo, rulElem;
        Node aimNode, Node, Node2, Node3;
        public Rules(Node Node, Node aimNode, int RuleNo)
        {
            rulElem = 1;
            this.Node = Node;
            this.aimNode = aimNode;
            this.RuleNo = RuleNo;
        }
        public int getRulElem()
        {
            return rulElem;
        }
        public Rules(Node Node, Node Node2, Node aimNode, int RuleNo)
        {
            rulElem = 2;
            this.Node = Node;
            this.Node2 = Node2;
            this.aimNode = aimNode;
            this.RuleNo = RuleNo;
        }
        public Rules(Node Node, Node Node2, Node Node3, Node aimNode, int RuleNo)
        {
            rulElem = 3;
            this.Node = Node;
            this.Node2 = Node2;
            this.Node3 = Node3;
            this.aimNode = aimNode;
            this.RuleNo = RuleNo;
        }
        public Node getAim()
        {
            return aimNode;
        }
        public Node getNode()
        {
            return Node;
        }
        public Node getNode2()
        {
            return Node2;
        }
        public Node getNode3()
        {
            return Node3;
        }
        public int getRuleNo()
        {
            return RuleNo;
        }

    }
}