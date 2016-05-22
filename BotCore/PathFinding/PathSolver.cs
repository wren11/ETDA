using BotCore.Types;
using System.Collections.Generic;

namespace BotCore.PathFinding
{
    public class PathSolver
    {
        public static List<PathFinderNode> FindPath(int[,] Matrix, Position Start, Position End)
        {
            var w = Matrix.GetLength(0);
            var h = Matrix.GetLength(1);

            try
            {
                Matrix[Start.X, Start.Y] = 0;
                Matrix[End.X, End.Y] = 0;

                var ClosedNodes = new bool[Matrix.GetUpperBound(0) + 1, Matrix.GetUpperBound(1) + 1];
                var Stack = new List<PathFinderNode>(new[] { new PathFinderNode { X = Start.X, Y = Start.Y, Heuristic = 0 } });
                var Heuristic = 1;

                PathFinderNode FinalNode = null;

                while ((FinalNode == null) && Stack.Count > 0)
                {
                    var NewStack = new List<PathFinderNode>();
                    for (var i = 0; i < Stack.Count; i++)
                    {
                        if (Stack[i].Heuristic > Heuristic)
                        {
                            NewStack.Add(Stack[i]);
                            continue;
                        }
                        if (Stack[i].X - 1 <= Matrix.GetUpperBound(0))
                            if (Stack[i].X - 1 >= 0)
                                if (!ClosedNodes[Stack[i].X - 1, Stack[i].Y])
                                    if (Matrix[Stack[i].X - 1, Stack[i].Y] != 1)
                                    {
                                        var LastNode = new PathFinderNode
                                        {
                                            LastNode = Stack[i].LastNode,
                                            X = Stack[i].X,
                                            Y = Stack[i].Y,
                                            Heuristic = Stack[i].Heuristic
                                        };
                                        var NewNode = new PathFinderNode
                                        {
                                            X = Stack[i].X - 1,
                                            Y = Stack[i].Y,
                                            NextNode = null,
                                            Heuristic = LastNode.Heuristic + (byte)Matrix[Stack[i].X - 1, Stack[i].Y]
                                        };
                                        LastNode.NextNode = NewNode;
                                        NewNode.LastNode = LastNode;
                                        if (Stack[i].X - 1 == End.X && Stack[i].Y == End.Y)
                                        {
                                            FinalNode = NewNode;
                                            break;
                                        }
                                        ClosedNodes[Stack[i].X - 1, Stack[i].Y] = true;
                                        NewStack.Add(NewNode);
                                    }
                        if (Stack[i].X + 1 <= Matrix.GetUpperBound(0))
                            if (Stack[i].X + 1 >= 0)
                                if (!ClosedNodes[Stack[i].X + 1, Stack[i].Y])
                                    if (Matrix[Stack[i].X + 1, Stack[i].Y] != 1)
                                    {
                                        var LastNode = new PathFinderNode
                                        {
                                            LastNode = Stack[i].LastNode,
                                            X = Stack[i].X,
                                            Y = Stack[i].Y,
                                            Heuristic = Stack[i].Heuristic
                                        };
                                        var NewNode = new PathFinderNode
                                        {
                                            X = Stack[i].X + 1,
                                            Y = Stack[i].Y,
                                            NextNode = null,
                                            Heuristic = LastNode.Heuristic + (byte)Matrix[Stack[i].X + 1, Stack[i].Y]
                                        };
                                        LastNode.NextNode = NewNode;
                                        NewNode.LastNode = LastNode;
                                        if (Stack[i].X + 1 == End.X && Stack[i].Y == End.Y)
                                        {
                                            FinalNode = NewNode;
                                            break;
                                        }
                                        ClosedNodes[Stack[i].X + 1, Stack[i].Y] = true;
                                        NewStack.Add(NewNode);
                                    }
                        if (Stack[i].Y - 1 <= Matrix.GetUpperBound(1))
                            if (Stack[i].Y - 1 >= 0)
                                if (!ClosedNodes[Stack[i].X, Stack[i].Y - 1])
                                    if (Matrix[Stack[i].X, Stack[i].Y - 1] != 1)
                                    {
                                        var LastNode = new PathFinderNode
                                        {
                                            LastNode = Stack[i].LastNode,
                                            X = Stack[i].X,
                                            Y = Stack[i].Y,
                                            Heuristic = Stack[i].Heuristic
                                        };
                                        var NewNode = new PathFinderNode
                                        {
                                            X = Stack[i].X,
                                            Y = Stack[i].Y - 1,
                                            NextNode = null,
                                            Heuristic = LastNode.Heuristic + (byte)Matrix[Stack[i].X, Stack[i].Y - 1]
                                        };
                                        LastNode.NextNode = NewNode;
                                        NewNode.LastNode = LastNode;
                                        if (Stack[i].X == End.X && Stack[i].Y - 1 == End.Y)
                                        {
                                            FinalNode = NewNode;
                                            break;
                                        }
                                        ClosedNodes[Stack[i].X, Stack[i].Y - 1] = true;
                                        NewStack.Add(NewNode);
                                    }
                        if (Stack[i].Y + 1 <= Matrix.GetUpperBound(1))
                            if (Stack[i].Y + 1 >= 0)
                                if (!ClosedNodes[Stack[i].X, Stack[i].Y + 1])
                                    if (Matrix[Stack[i].X, Stack[i].Y + 1] != 1)
                                    {
                                        var LastNode = new PathFinderNode
                                        {
                                            LastNode = Stack[i].LastNode,
                                            X = Stack[i].X,
                                            Y = Stack[i].Y,
                                            Heuristic = Stack[i].Heuristic
                                        };
                                        var NewNode = new PathFinderNode
                                        {
                                            X = Stack[i].X,
                                            Y = Stack[i].Y + 1,
                                            NextNode = null,
                                            Heuristic = LastNode.Heuristic + (byte)Matrix[Stack[i].X, Stack[i].Y + 1]
                                        };
                                        LastNode.NextNode = NewNode;
                                        NewNode.LastNode = LastNode;
                                        if (Stack[i].X == End.X && Stack[i].Y + 1 == End.Y)
                                        {
                                            FinalNode = NewNode;
                                            break;
                                        }
                                        ClosedNodes[Stack[i].X, Stack[i].Y + 1] = true;
                                        NewStack.Add(NewNode);
                                    }
                    }
                    Heuristic++;
                    Stack = NewStack;
                }
                if (FinalNode != null)
                {
                    Stack = new List<PathFinderNode>();
                    while (FinalNode != null)
                    {
                        Stack.Add(FinalNode);
                        FinalNode = FinalNode.LastNode;
                    }
                    Stack.Reverse();
                    return Stack;
                }
                return null;
            }
            catch
            {
                System.Console.WriteLine("Map Dimensions are fucked.");
                return null;
            }
        }

        public class PathFinderNode
        {
            public PathFinderNode LastNode;
            public PathFinderNode NextNode;
            public int X { get; set; }
            public int Y { get; set; }
            public int Heuristic { get; set; }
        }
    }
}
