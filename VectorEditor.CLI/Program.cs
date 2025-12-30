using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

var rootLayer = new Layer("Root Canvas");
var layer1 = new Layer("Layer 1");
var layer2 = new Layer("Layer 2");

rootLayer.Add(layer1);
rootLayer.Add(layer2);

var canvas1 = new List<IShape>();
var canvas2 = new List<IShape>();

var builder = new LineBuilder("black", 2)
    .SetStart(new Point(0, 0))
    .SetEnd(new Point(5, 5));

var circleBuilder = new CircleBuilder("red", "blue", 3)
    .SetStart(new Point(10, 10))
    .SetRadius(5);

var line2Builder = new LineBuilder("green", 3)
    .SetStart(new Point(20, 20))
    .SetEnd(new Point(30, 30));

var cmdManager = new CommandManager();

cmdManager.Execute(new AddShapeCommand(builder, canvas1));
layer1.Add(canvas1[0]);
Console.WriteLine("Canvas after adding line to Layer 1:");
rootLayer.ConsoleDisplay();
Console.WriteLine();

cmdManager.Execute(new AddShapeCommand(circleBuilder, canvas1));
layer1.Add(canvas1[1]);
Console.WriteLine("Canvas after adding circle to Layer 1:");
rootLayer.ConsoleDisplay();
Console.WriteLine();

cmdManager.Execute(new AddShapeCommand(line2Builder, canvas2));
layer2.Add(canvas2[0]);
Console.WriteLine("Canvas after adding line to Layer 2:");
rootLayer.ConsoleDisplay();
Console.WriteLine();

cmdManager.Undo();
layer2.Remove(canvas2[0]);
canvas2.RemoveAt(canvas2.Count - 1);
Console.WriteLine("Canvas after undo:");
rootLayer.ConsoleDisplay();
Console.WriteLine();

cmdManager.Redo();
layer2.Add(canvas2[0]);
Console.WriteLine("Canvas after redo:");
rootLayer.ConsoleDisplay();