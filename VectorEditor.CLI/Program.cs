using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;


// test buildera
/*var rootLayer = new Layer("Root Canvas");
var layer1 = new Layer("Layer 1");
var layer2 = new Layer("Layer 2");

rootLayer.Add(layer1);
rootLayer.Add(layer2);

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

cmdManager.Execute(new AddShapeCommand(builder, layer1));
Console.WriteLine("Canvas after adding line to Layer 1:");
rootLayer.ConsoleDisplay();
Console.WriteLine();

cmdManager.Execute(new AddShapeCommand(circleBuilder, layer1));
Console.WriteLine("Canvas after adding circle to Layer 1:");
rootLayer.ConsoleDisplay();
Console.WriteLine();

cmdManager.Execute(new AddShapeCommand(line2Builder, layer2));
Console.WriteLine("Canvas after adding line to Layer 2:");
rootLayer.ConsoleDisplay();
Console.WriteLine();

cmdManager.Undo();
Console.WriteLine("Canvas after undo:");
rootLayer.ConsoleDisplay();
Console.WriteLine();

cmdManager.Redo();
Console.WriteLine("Canvas after redo:");
rootLayer.ConsoleDisplay();
*/



// test prostego zaznaczania
/*
// 1. Przygotowanie warstwy i obiektów
var testLayer = new Layer("Selection Test Layer");

// Okrąg: Środek (20, 20), Promień 10
var circle = new Circle(new Point(20, 20), 10, "black", "red", 2);

// Linia 1: Całkowicie wewnątrz obszaru (25, 25) do (35, 35)
var lineInside = new Line(new Point(25, 25), new Point(35, 35), "blue", 2);

// Linia 2: Całkowicie poza obszarem (100, 100) do (110, 110)
var lineOutside = new Line(new Point(100, 100), new Point(110, 110), "green", 1);

testLayer.Add(circle);
testLayer.Add(lineInside);
testLayer.Add(lineOutside);

// 2. Wykonanie testu zaznaczania (GroupCommand)
// Definiujemy obszar zaznaczenia od (15, 15) do (40, 40)
// Powinien złapać:
// - Okrąg (bo jego fragment/środek jest w tym obszarze)
// - LineInside (bo oba punkty są wewnątrz)
// Nie powinien złapać:
// - LineOutside
var p1 = new Point(15, 15);
var p2 = new Point(100, 99);

Console.WriteLine("=== URUCHOMIENIE TESTU ZAZNACZANIA ===");
var groupCmd = new GroupCommand(testLayer, p1, p2);
groupCmd.Execute();

// 3. Weryfikacja wizualna całego Layera
Console.WriteLine("=== PEŁNA ZAWARTOŚĆ LAYERA (DLA PORÓWNANIA) ===");
testLayer.ConsoleDisplay();
*/

// --- PRZYGOTOWANIE STRUKTURY ---

var rootLayer = new Layer("World");
var headLayer = new Layer("Head Layer");
var bodyLayer = new Layer("Body Layer");

rootLayer.Add(headLayer);
rootLayer.Add(bodyLayer);

// 1. Elementy Głowy (Centrum ok. 50, 50)
headLayer.Add(new Circle(new Point(50, 50), 20, "skin", "black", 2)); // Głowa
headLayer.Add(new Circle(new Point(42, 45), 3, "white", "black", 1));  // Lewe oko
headLayer.Add(new Circle(new Point(58, 45), 3, "white", "black", 1));  // Prawe oko
headLayer.Add(new Triangle(new Point(50, 48), new Point(48, 55), new Point(52, 55), "red", "black", 2)); // Nos
headLayer.Add(new Rectangle(new Point(40, 60), new Point(60, 65), "pink", "black", 2)); // Usta

// 2. Elementy Ciała (T-pos)
bodyLayer.Add(new Rectangle(new Point(40, 70), new Point(60, 120), "blue", "black", 2));  // Tułów
bodyLayer.Add(new Rectangle(new Point(10, 80), new Point(40, 90), "skin", "black", 2));   // Lewa ręka
bodyLayer.Add(new Rectangle(new Point(60, 80), new Point(90, 90), "skin", "black", 2));   // Prawa ręka
bodyLayer.Add(new Rectangle(new Point(40, 120), new Point(48, 160), "jeans", "black", 2)); // Lewa noga
bodyLayer.Add(new Rectangle(new Point(52, 120), new Point(60, 160), "jeans", "black", 2)); // Prawa noga

rootLayer.Add(new Rectangle(new Point(2, 2), new Point(3, 3), "white", "black", 1));
var cmdManager = new CommandManager();

// --- TEST 1: ZAZNACZENIE SAMEJ GÓRY GŁOWY ---
// Obszar od (30, 20) do (70, 40) - powinien dotknąć tylko głównego okręgu głowy
Console.WriteLine(">>> TEST 1: ZAZNACZENIE CZUBKA GŁOWY <<<");
var selectHeadTop = new GroupCommand(rootLayer, new Point(30, 20), new Point(70, 40));
cmdManager.Execute(selectHeadTop);

// --- TEST 2: ZAZNACZENIE CAŁEGO CZŁOWIEKA ---
// Obszar od (0, 0) do (200, 200) - powinien wypisać obie warstwy i wszystkie ich dzieci
Console.WriteLine("\n>>> TEST 2: ZAZNACZENIE WSZYSTKIEGO <<<");
var selectAll = new GroupCommand(rootLayer, new Point(0, 0), new Point(200, 200));
cmdManager.Execute(selectAll);

// --- TEST 3: ZAZNACZENIE TYLKO PRAWEJ RĘKI ---
// Obszar od (70, 75) do (100, 100)
Console.WriteLine("\n>>> TEST 3: ZAZNACZENIE PRAWEJ RĘKI <<<");
var selectHand = new GroupCommand(rootLayer, new Point(70, 75), new Point(100, 100));
cmdManager.Execute(selectHand);