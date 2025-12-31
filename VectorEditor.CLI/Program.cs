using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

Console.WriteLine("Hello, World!");

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

// test grupowania i strategii
/*
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
selectHeadTop.DisplayResults();

// --- TEST 2: ZAZNACZENIE CAŁEGO CZŁOWIEKA ---
// Obszar od (0, 0) do (200, 200) - powinien wypisać obie warstwy i wszystkie ich dzieci
Console.WriteLine("\n>>> TEST 2: ZAZNACZENIE WSZYSTKIEGO <<<");
var selectAll = new GroupCommand(rootLayer, new Point(0, 0), new Point(200, 200));
cmdManager.Execute(selectAll);
selectAll.DisplayResults();

// --- TEST 3: ZAZNACZENIE TYLKO PRAWEJ RĘKI ---
// Obszar od (70, 75) do (100, 100)
Console.WriteLine("\n>>> TEST 3: ZAZNACZENIE PRAWEJ RĘKI <<<");
var selectHand = new GroupCommand(rootLayer, new Point(70, 75), new Point(100, 100));
cmdManager.Execute(selectHand);
selectHand.DisplayResults();

// --- TEST 4: ZMIANA KOLORÓW PRZY UŻYCIU STRATEGII ---
Console.WriteLine("\n>>> TEST 4: ZMIANA KOLORÓW W HEAD LAYER <<<");
Console.WriteLine("Canvas przed zmianą kolorów:");
rootLayer.ConsoleDisplay();
Console.WriteLine();

// Tworzymy strategię zmieniającą kolor wypełnienia na "yellow"
var colorStrategy = new VectorEditor.Core.Strategy.ChangeContourColorStrategy("yellow");
var applyColorCmd = new ApplyStrategyCommand(colorStrategy, headLayer);
cmdManager.Execute(applyColorCmd);

Console.WriteLine("Canvas po zmianie kolorów:");
rootLayer.ConsoleDisplay();
Console.WriteLine();

// --- TEST 5: COFNIĘCIE ZMIANY KOLORÓW ---
Console.WriteLine("\n>>> TEST 5: COFNIĘCIE ZMIANY KOLORÓW (UNDO) <<<");
cmdManager.Undo();
Console.WriteLine("Canvas po cofnięciu zmiany kolorów:");
rootLayer.ConsoleDisplay();
*/

// --- TEST 6: ZAAWANSOWANE ZAZNACZENIE I STRATEGIA ---
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


Console.WriteLine("\n>>> TEST 6: ZMIANA KOLORU WYBRANYCH ELEMENTÓW (NOGI) <<<");

// 1. Definiujemy obszar nóg (zgodnie ze strukturą bodyLayer)
// Lewa noga: (40, 120) do (48, 160)
// Prawa noga: (52, 120) do (60, 160)
var pStart = new Point(35, 115);
var pEnd = new Point(65, 170);

// 2. Wykonujemy zaznaczenie (używamy bodyLayer jako celu dla precyzji)
var selectLegsCmd = new GroupCommand(bodyLayer, pStart, pEnd);
cmdManager.Execute(selectLegsCmd);
selectLegsCmd.DisplayResults();

// 3. Aplikujemy strategię tylko na to, co znalazło GroupCommand
// Wykorzystujemy nowy konstruktor ApplyStrategyCommand przyjmujący IEnumerable<ICanvas>
var greenContour = new VectorEditor.Core.Strategy.ChangeContourColorStrategy("green");
var applyGreenCmd = new ApplyStrategyCommand(greenContour, selectLegsCmd.FoundElements);

Console.WriteLine("Aplikuję zielony kontur na znalezione elementy...");
cmdManager.Execute(applyGreenCmd);

Console.WriteLine("\nStan po aplikacji strategii na zaznaczenie:");
rootLayer.ConsoleDisplay();

// --- TEST 7: UNDO STRATEGII NA ZAZNACZENIU ---
Console.WriteLine("\n>>> TEST 7: UNDO STRATEGII NA ZAZNACZENIU <<<");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO (nogi powinny wrócić do koloru 'black'):");
rootLayer.ConsoleDisplay();

// --- TEST 8: ZMIANA KOLORU ZAWARTOŚCI WYBRANYCH ELEMENTÓW GŁOWY ---
Console.WriteLine("\n>>> TEST 8: ZMIANA KOLORU ZAWARTOŚCI (OKA I NOS) <<<");

// 1. Zaznaczamy obszar oczu i nosa (42, 40) do (60, 56)
var selectFaceCmd = new GroupCommand(headLayer, new Point(40, 40), new Point(60, 56));
cmdManager.Execute(selectFaceCmd);
selectFaceCmd.DisplayResults();

// 2. Zmieniamy kolor zawartości na "cyan"
var cyanContent = new VectorEditor.Core.Strategy.ChangeContentColorStrategy("cyan");
var applyCyanCmd = new ApplyStrategyCommand(cyanContent, selectFaceCmd.FoundElements);

Console.WriteLine("Aplikuję cyan jako kolor zawartości na znalezione elementy...");
cmdManager.Execute(applyCyanCmd);

Console.WriteLine("\nStan po zmianie koloru zawartości:");
rootLayer.ConsoleDisplay();

// --- TEST 9: UNDO ZMIANY KOLORU ZAWARTOŚCI ---
Console.WriteLine("\n>>> TEST 9: UNDO ZMIANY KOLORU ZAWARTOŚCI <<<");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO (oczy i nos powinny wrócić do oryginalnych kolorów):");
rootLayer.ConsoleDisplay();

// --- TEST 10: ZMIANA KOLORÓW CAŁEJ WARSTWY CIAŁA ---
Console.WriteLine("\n>>> TEST 10: ZMIANA KONTURU I ZAWARTOŚCI CAŁEGO BODY LAYER <<<");

Console.WriteLine("Stan przed zmianami:");
bodyLayer.ConsoleDisplay();

// 1. Zmiana konturu całej warstwy na "red"
var redContour = new VectorEditor.Core.Strategy.ChangeContourColorStrategy("red");
var applyRedContourCmd = new ApplyStrategyCommand(redContour, bodyLayer);
cmdManager.Execute(applyRedContourCmd);

Console.WriteLine("\nStan po zmianie konturu na 'red':");
bodyLayer.ConsoleDisplay();

// 2. Zmiana zawartości całej warstwy na "orange"
var orangeContent = new VectorEditor.Core.Strategy.ChangeContentColorStrategy("orange");
var applyOrangeContentCmd = new ApplyStrategyCommand(orangeContent, bodyLayer);
cmdManager.Execute(applyOrangeContentCmd);

Console.WriteLine("\nStan po zmianie zawartości na 'orange':");
bodyLayer.ConsoleDisplay();

// --- TEST 11: WIELOKROTNE UNDO ---
Console.WriteLine("\n>>> TEST 11: WIELOKROTNE UNDO (SPRAWDZENIE STOSU KOMEND) <<<");

Console.WriteLine("Wykonuję pierwsze UNDO (cofam zmianę zawartości):");
cmdManager.Undo();
bodyLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję drugie UNDO (cofam zmianę konturu):");
cmdManager.Undo();
bodyLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję trzecie UNDO (cofam zaznaczenie z TEST 8):");
cmdManager.Undo();

Console.WriteLine("\nWykonuję czwarte UNDO (cofam zaznaczenie z TEST 6/7):");
cmdManager.Undo();

Console.WriteLine("\nKońcowy stan całej sceny:");
rootLayer.ConsoleDisplay();
