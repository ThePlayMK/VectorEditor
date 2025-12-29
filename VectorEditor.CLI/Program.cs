using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

var canvas = new List<IShape>();

var builder = new LineBuilder("black", 2)
    .SetStart(new Point(0, 0))
    .SetEnd(new Point(5, 5));

var cmdManager = new CommandManager();

cmdManager.Execute(new AddShapeCommand(builder, canvas));
Console.WriteLine($"Canvas after add: {string.Join(", ", canvas.Select(s => s.ToString()))}");

cmdManager.Undo();
Console.WriteLine($"Canvas after undo: {string.Join(", ", canvas.Select(s => s.ToString()))}");

cmdManager.Redo();
Console.WriteLine($"Canvas after redo: {string.Join(", ", canvas.Select(s => s.ToString()))}");