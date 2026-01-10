using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.State;
using VectorEditor.Core.Strategy;
using VectorEditor.Core.Structures;

Console.WriteLine("Hello, World!");

// test buildera
/*
var rootLayer = new Layer("Root Canvas");
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

// 2. Wykonanie testu zaznaczania (SelectionCommand)
// Definiujemy obszar zaznaczenia od (15, 15) do (40, 40)
// Powinien złapać:
// - Okrąg (bo jego fragment/środek jest w tym obszarze)
// - LineInside (bo oba punkty są wewnątrz)
// Nie powinien złapać:
// - LineOutside
var p1 = new Point(15, 15);
var p2 = new Point(100, 100);

Console.WriteLine("=== URUCHOMIENIE TESTU ZAZNACZANIA ===");
var groupCmd = new SelectionCommand(testLayer, p1, p2);
groupCmd.Execute();
groupCmd.DisplayResults();

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
var selectHeadTop = new SelectionCommand(rootLayer, new Point(30, 20), new Point(70, 40));
cmdManager.Execute(selectHeadTop);
selectHeadTop.DisplayResults();

// --- TEST 2: ZAZNACZENIE CAŁEGO CZŁOWIEKA ---
// Obszar od (0, 0) do (200, 200) - powinien wypisać obie warstwy i wszystkie ich dzieci
Console.WriteLine("\n>>> TEST 2: ZAZNACZENIE WSZYSTKIEGO <<<");
var selectAll = new SelectionCommand(rootLayer, new Point(0, 0), new Point(200, 200));
cmdManager.Execute(selectAll);
selectAll.DisplayResults();

// --- TEST 3: ZAZNACZENIE TYLKO PRAWEJ RĘKI ---
// Obszar od (70, 75) do (100, 100)
Console.WriteLine("\n>>> TEST 3: ZAZNACZENIE PRAWEJ RĘKI <<<");
var selectHand = new SelectionCommand(bodyLayer, new Point(70, 75), new Point(100, 100));
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
/*
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
var selectLegsCmd = new SelectionCommand(bodyLayer, pStart, pEnd);
cmdManager.Execute(selectLegsCmd);
selectLegsCmd.DisplayResults();

// 3. Aplikujemy strategię tylko na to, co znalazło SelectionCommand
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
var selectFaceCmd = new SelectionCommand(headLayer, new Point(40, 40), new Point(60, 56));
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
*/

// --- TEST 12: USUWANIE POJEDYNCZEGO ELEMENTU PRZEZ STRATEGIĘ ---
/*
var rootLayer = new Layer("World");
var testLayer = new Layer("Test Layer");
rootLayer.Add(testLayer);

var circle1 = new Circle(new Point(10, 10), 5, "red", "black", 1);
var circle2 = new Circle(new Point(20, 20), 5, "blue", "black", 1);
var line1 = new Line(new Point(30, 30), new Point(40, 40), "green", 2);

testLayer.Add(circle1);
testLayer.Add(circle2);
testLayer.Add(line1);

var cmdManager = new CommandManager();

Console.WriteLine(">>> TEST 12: USUWANIE POJEDYNCZEGO ELEMENTU PRZEZ STRATEGIĘ <<<");
Console.WriteLine("Stan początkowy:");
testLayer.ConsoleDisplay();

var removeStrategy = new RemoveStrategy();
var removeCircle1Cmd = new ApplyStrategyCommand(removeStrategy, circle1);
cmdManager.Execute(removeCircle1Cmd);

Console.WriteLine("\nStan po usunięciu circle1:");
testLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO:");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO:");
testLayer.ConsoleDisplay();

// --- TEST 13: USUWANIE ZGRUPOWANYCH ELEMENTÓW PRZEZ STRATEGIĘ ---
Console.WriteLine("\n>>> TEST 13: USUWANIE ZGRUPOWANYCH ELEMENTÓW PRZEZ STRATEGIĘ <<<");

var bodyLayer = new Layer("Body Layer");
rootLayer.Add(bodyLayer);

bodyLayer.Add(new Rectangle(new Point(40, 70), new Point(60, 120), "blue", "black", 2));
bodyLayer.Add(new Rectangle(new Point(10, 80), new Point(40, 90), "skin", "black", 2));
bodyLayer.Add(new Rectangle(new Point(60, 80), new Point(90, 90), "skin", "black", 2));
bodyLayer.Add(new Rectangle(new Point(40, 120), new Point(48, 160), "jeans", "black", 2));
bodyLayer.Add(new Rectangle(new Point(52, 120), new Point(60, 160), "jeans", "black", 2));

Console.WriteLine("Stan początkowy bodyLayer:");
bodyLayer.ConsoleDisplay();

var selectLegsCmd = new SelectionCommand(bodyLayer, new Point(35, 115), new Point(65, 170));
cmdManager.Execute(selectLegsCmd);

Console.WriteLine("\nZnalezione elementy (nogi):");
selectLegsCmd.DisplayResults();

var removeGroupStrategy = new RemoveStrategy();
var removeGroupCmd = new ApplyStrategyCommand(removeGroupStrategy, selectLegsCmd.FoundElements);
cmdManager.Execute(removeGroupCmd);

Console.WriteLine("\nStan po usunięciu zgrupowanych elementów:");
bodyLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO:");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO:");
bodyLayer.ConsoleDisplay();

// --- TEST 14: USUWANIE Z ZAGNIEŻDŻONYCH WARSTW PRZEZ STRATEGIĘ ---
Console.WriteLine("\n>>> TEST 14: USUWANIE Z ZAGNIEŻDŻONYCH WARSTW PRZEZ STRATEGIĘ <<<");

var parentLayer = new Layer("Parent");
var childLayer1 = new Layer("Child1");
var childLayer2 = new Layer("Child2");

parentLayer.Add(childLayer1);
parentLayer.Add(childLayer2);

childLayer1.Add(new Circle(new Point(50, 50), 10, "yellow", "black", 1));
childLayer1.Add(new Circle(new Point(70, 50), 10, "orange", "black", 1));
childLayer2.Add(new Line(new Point(100, 100), new Point(150, 150), "purple", 3));

Console.WriteLine("Stan początkowy:");
parentLayer.ConsoleDisplay();

var selectAllCmd = new SelectionCommand(parentLayer, new Point(0, 0), new Point(200, 200));
cmdManager.Execute(selectAllCmd);

Console.WriteLine("\nZnalezione wszystkie elementy:");
selectAllCmd.DisplayResults();

var removeAllStrategy = new RemoveStrategy();
var removeAllCmd = new ApplyStrategyCommand(removeAllStrategy, selectAllCmd.FoundElements);
cmdManager.Execute(removeAllCmd);

Console.WriteLine("\nStan po usunięciu wszystkich elementów:");
parentLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO:");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO:");
parentLayer.ConsoleDisplay();

// --- TEST 15: WIELOKROTNE USUWANIE I UNDO PRZEZ STRATEGIĘ ---
Console.WriteLine("\n>>> TEST 15: WIELOKROTNE USUWANIE I UNDO PRZEZ STRATEGIĘ <<<");

var multiLayer = new Layer("Multi Layer");
var shape1 = new Triangle(new Point(10, 10), new Point(20, 10), new Point(15, 20), "red", "black", 1);
var shape2 = new Triangle(new Point(30, 10), new Point(40, 10), new Point(35, 20), "green", "black", 1);
var shape3 = new Triangle(new Point(50, 10), new Point(60, 10), new Point(55, 20), "blue", "black", 1);

multiLayer.Add(shape1);
multiLayer.Add(shape2);
multiLayer.Add(shape3);

Console.WriteLine("Stan początkowy:");
multiLayer.ConsoleDisplay();

var removeStrategy1 = new RemoveStrategy();
var remove1 = new ApplyStrategyCommand(removeStrategy1, shape1);
var removeStrategy2 = new RemoveStrategy();
var remove2 = new ApplyStrategyCommand(removeStrategy2, shape2);

cmdManager.Execute(remove1);
Console.WriteLine("\nPo usunięciu shape1:");
multiLayer.ConsoleDisplay();

cmdManager.Execute(remove2);
Console.WriteLine("\nPo usunięciu shape2:");
multiLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję pierwsze UNDO:");
cmdManager.Undo();
multiLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję drugie UNDO:");
cmdManager.Undo();
multiLayer.ConsoleDisplay();
*/

// --- TEST 16: BLOKOWANIE POJEDYNCZEGO KSZTAŁTU PRZEZ STRATEGIĘ ---
/*
var rootLayer = new Layer("World");
var testLayer = new Layer("Test Layer");
rootLayer.Add(testLayer);

var blockedCircle = new Circle(new Point(10, 10), 5, "red", "black", 1);
var unblockedCircle = new Circle(new Point(30, 30), 5, "blue", "black", 1);

testLayer.Add(blockedCircle);
testLayer.Add(unblockedCircle);

var cmdManager = new CommandManager();

Console.WriteLine(">>> TEST 16: BLOKOWANIE POJEDYNCZEGO KSZTAŁTU PRZEZ STRATEGIĘ <<<");
Console.WriteLine("Stan początkowy:");
testLayer.ConsoleDisplay();

// Blokujemy pierwszy okrąg za pomocą strategii
var blockStrategy = new BlockCanvasStrategy();
var blockCircleCmd = new ApplyStrategyCommand(blockStrategy, blockedCircle);
cmdManager.Execute(blockCircleCmd);

Console.WriteLine($"\nZablokowano blockedCircle za pomocą BlockCanvasStrategy (IsBlocked = {blockedCircle.IsBlocked})");

// Próbujemy zmienić kolor konturu obu okręgów
var redContour = new ChangeContourColorStrategy("red");
var applyRedCmd = new ApplyStrategyCommand(redContour, testLayer);
cmdManager.Execute(applyRedCmd);

Console.WriteLine("\nStan po próbie zmiany koloru konturu (zablokowany nie powinien się zmienić):");
testLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO (cofam zmianę koloru):");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO zmiany koloru:");
testLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO (cofam blokadę):");
cmdManager.Undo();

Console.WriteLine($"Stan po UNDO blokady (IsBlocked = {blockedCircle.IsBlocked}):");
testLayer.ConsoleDisplay();

// --- TEST 17: BLOKOWANIE CAŁEJ WARSTWY PRZEZ STRATEGIĘ ---
Console.WriteLine("\n>>> TEST 17: BLOKOWANIE CAŁEJ WARSTWY PRZEZ STRATEGIĘ <<<");

var blockedLayer = new Layer("Blocked Layer");
blockedLayer.Add(new Rectangle(new Point(50, 50), new Point(70, 70), "green", "black", 2));
blockedLayer.Add(new Triangle(new Point(80, 80), new Point(90, 80), new Point(85, 90), "yellow", "black", 1));

rootLayer.Add(blockedLayer);

Console.WriteLine("Stan przed zablokowaniem warstwy:");
blockedLayer.ConsoleDisplay();

// Blokujemy całą warstwę za pomocą strategii
var blockLayerStrategy = new BlockCanvasStrategy();
var blockLayerCmd = new ApplyStrategyCommand(blockLayerStrategy, blockedLayer);
cmdManager.Execute(blockLayerCmd);

Console.WriteLine($"\nZablokowano blockedLayer za pomocą BlockCanvasStrategy (IsBlocked = {blockedLayer.IsBlocked})");

// Próbujemy zmienić kolor zawartości
var purpleContent = new ChangeContentColorStrategy("purple");
var applyPurpleCmd = new ApplyStrategyCommand(purpleContent, blockedLayer);
cmdManager.Execute(applyPurpleCmd);

Console.WriteLine("\nStan po próbie zmiany koloru zawartości (zablokowana warstwa nie powinna się zmienić):");
blockedLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO (cofam próbę zmiany koloru):");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO:");
blockedLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO (cofam blokadę warstwy):");
cmdManager.Undo();

Console.WriteLine($"Stan po UNDO blokady (IsBlocked = {blockedLayer.IsBlocked}):");
blockedLayer.ConsoleDisplay();

// --- TEST 18: CZĘŚCIOWE BLOKOWANIE W ZAZNACZENIU PRZEZ STRATEGIĘ ---
Console.WriteLine("\n>>> TEST 18: CZĘŚCIOWE BLOKOWANIE W ZAZNACZENIU PRZEZ STRATEGIĘ <<<");

var mixedLayer = new Layer("Mixed Layer");
var shape1 = new Circle(new Point(100, 100), 10, "cyan", "black", 1);
var shape2 = new Circle(new Point(120, 100), 10, "magenta", "black", 1);
var shape3 = new Circle(new Point(140, 100), 10, "orange", "black", 1);

mixedLayer.Add(shape1);
mixedLayer.Add(shape2);
mixedLayer.Add(shape3);

rootLayer.Add(mixedLayer);

Console.WriteLine("Stan początkowy mixedLayer:");
mixedLayer.ConsoleDisplay();

// Blokujemy środkowy okrąg za pomocą strategii
var blockShape2Strategy = new BlockCanvasStrategy();
var blockShape2Cmd = new ApplyStrategyCommand(blockShape2Strategy, shape2);
cmdManager.Execute(blockShape2Cmd);

Console.WriteLine($"\nZablokowano shape2 za pomocą BlockCanvasStrategy (IsBlocked = {shape2.IsBlocked})");

// Zaznaczamy wszystkie trzy okręgi
var selectAllCmd = new SelectionCommand(mixedLayer, new Point(90, 90), new Point(150, 110));
cmdManager.Execute(selectAllCmd);

Console.WriteLine("\nZnalezione elementy w zaznaczeniu:");
selectAllCmd.DisplayResults();

// Aplikujemy strategię zmiany konturu na zaznaczenie
var whiteContour = new ChangeContourColorStrategy("white");
var applyWhiteCmd = new ApplyStrategyCommand(whiteContour, selectAllCmd.FoundElements);
cmdManager.Execute(applyWhiteCmd);

Console.WriteLine("\nStan po aplikacji strategii (shape2 nie powinien się zmienić):");
mixedLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO (cofam zmianę koloru):");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO:");
mixedLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO (cofam zaznaczenie):");
cmdManager.Undo();

Console.WriteLine("\nWykonuję UNDO (cofam blokadę shape2):");
cmdManager.Undo();

Console.WriteLine($"Stan po cofnięciu blokady (IsBlocked = {shape2.IsBlocked}):");
mixedLayer.ConsoleDisplay();

// --- TEST 19: ODBLOKOWYWANIE I PONOWNA MODYFIKACJA PRZEZ STRATEGIĘ ---
Console.WriteLine("\n>>> TEST 19: ODBLOKOWYWANIE I PONOWNA MODYFIKACJA PRZEZ STRATEGIĘ <<<");

var unblockTestLayer = new Layer("Unblock Test Layer");
var testShape = new Rectangle(new Point(200, 200), new Point(220, 220), "brown", "black", 2);
unblockTestLayer.Add(testShape);
rootLayer.Add(unblockTestLayer);

Console.WriteLine("Stan początkowy:");
unblockTestLayer.ConsoleDisplay();

// Blokujemy kształt za pomocą strategii
var blockTestShapeStrategy = new BlockCanvasStrategy();
var blockTestShapeCmd = new ApplyStrategyCommand(blockTestShapeStrategy, testShape);
cmdManager.Execute(blockTestShapeCmd);

Console.WriteLine($"\nZablokowano testShape za pomocą BlockCanvasStrategy (IsBlocked = {testShape.IsBlocked})");

// Próbujemy zmienić kolor
var pinkContent = new ChangeContentColorStrategy("pink");
var applyPink1 = new ApplyStrategyCommand(pinkContent, testShape);
cmdManager.Execute(applyPink1);

Console.WriteLine("\nStan po próbie zmiany koloru (zablokowany - bez zmian):");
unblockTestLayer.ConsoleDisplay();

// Odblokowujemy kształt za pomocą strategii
var unblockStrategy = new UnblockCanvasStrategy();
var unblockTestShapeCmd = new ApplyStrategyCommand(unblockStrategy, testShape);
cmdManager.Execute(unblockTestShapeCmd);

Console.WriteLine($"\nOdblokowano testShape za pomocą UnblockCanvasStrategy (IsBlocked = {testShape.IsBlocked})");

// Ponownie próbujemy zmienić kolor
var applyPink2 = new ApplyStrategyCommand(pinkContent, testShape);
cmdManager.Execute(applyPink2);

Console.WriteLine("\nStan po zmianie koloru (odblokowany - kolor zmieniony):");
unblockTestLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO (cofam zmianę koloru):");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO:");
unblockTestLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO (cofam odblokowanie):");
cmdManager.Undo();

Console.WriteLine($"Stan po cofnięciu odblokowania (IsBlocked = {testShape.IsBlocked}):");
unblockTestLayer.ConsoleDisplay();

// --- TEST 20: ZAGNIEŻDŻONE WARSTWY Z BLOKOWANIEM PRZEZ STRATEGIĘ ---
Console.WriteLine("\n>>> TEST 20: ZAGNIEŻDŻONE WARSTWY Z BLOKOWANIEM PRZEZ STRATEGIĘ <<<");

var parentBlockedLayer = new Layer("Parent Blocked");
var childUnblockedLayer = new Layer("Child Unblocked");

parentBlockedLayer.Add(childUnblockedLayer);
rootLayer.Add(parentBlockedLayer);

childUnblockedLayer.Add(new Line(new Point(300, 300), new Point(350, 350), "violet", 3));
childUnblockedLayer.Add(new Line(new Point(300, 350), new Point(350, 300), "indigo", 3));

Console.WriteLine("Stan początkowy:");
parentBlockedLayer.ConsoleDisplay();

// Blokujemy warstwę nadrzędną za pomocą strategii (dziecko pozostaje odblokowane)
var blockParentStrategy = new BlockCanvasStrategy();
var blockParentCmd = new ApplyStrategyCommand(blockParentStrategy, parentBlockedLayer);
cmdManager.Execute(blockParentCmd);

Console.WriteLine($"\nZablokowano parentBlockedLayer za pomocą BlockCanvasStrategy (IsBlocked = {parentBlockedLayer.IsBlocked})");
Console.WriteLine($"childUnblockedLayer.IsBlocked = {childUnblockedLayer.IsBlocked}");

// Próbujemy zmienić kolor na warstwie nadrzędnej
var grayContour = new ChangeContourColorStrategy("gray");
var applyGrayCmd = new ApplyStrategyCommand(grayContour, parentBlockedLayer);
cmdManager.Execute(applyGrayCmd);

Console.WriteLine("\nStan po próbie zmiany koloru (zablokowana warstwa nadrzędna blokuje dostęp do dzieci):");
parentBlockedLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO (cofam próbę zmiany koloru):");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO:");
parentBlockedLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO (cofam blokadę warstwy nadrzędnej):");
cmdManager.Undo();

Console.WriteLine($"Stan po cofnięciu blokady (IsBlocked = {parentBlockedLayer.IsBlocked}):");
parentBlockedLayer.ConsoleDisplay();

Console.WriteLine("\n>>> KONIEC TESTÓW BLOKOWANIA/ODBLOKOWYWANIA PRZEZ STRATEGIĘ <<<");
*/

// --- TEST 21: UKRYWANIE POJEDYNCZEGO KSZTAŁTU ---
/*
var rootLayer = new Layer("World");
var testLayer = new Layer("Test Layer");
rootLayer.Add(testLayer);

var circle1 = new Circle(new Point(10, 10), 5, "red", "black", 1);
var circle2 = new Circle(new Point(20, 20), 5, "blue", "black", 1);

testLayer.Add(circle1);
testLayer.Add(circle2);

var cmdManager = new CommandManager();

Console.WriteLine(">>> TEST 21: UKRYWANIE POJEDYNCZEGO KSZTAŁTU <<<");
Console.WriteLine("Stan początkowy:");
testLayer.ConsoleDisplay();

var hideStrategy = new HideCanvasStrategy();
var hideCircle1Cmd = new ApplyStrategyCommand(hideStrategy, circle1);
cmdManager.Execute(hideCircle1Cmd);

Console.WriteLine("\nStan po ukryciu circle1:");
testLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO:");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO:");
testLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję REDO:");
cmdManager.Redo();

Console.WriteLine("Stan po REDO:");
testLayer.ConsoleDisplay();

// --- TEST 22: UKRYWANIE CAŁEJ WARSTWY ---
Console.WriteLine("\n>>> TEST 22: UKRYWANIE CAŁEJ WARSTWY <<<");

var hiddenLayer = new Layer("Hidden Layer");
hiddenLayer.Add(new Rectangle(new Point(50, 50), new Point(70, 70), "green", "black", 2));
hiddenLayer.Add(new Triangle(new Point(80, 80), new Point(90, 80), new Point(85, 90), "yellow", "black", 1));

rootLayer.Add(hiddenLayer);

Console.WriteLine("Stan przed ukryciem warstwy:");
hiddenLayer.ConsoleDisplay();

var hideLayerStrategy = new HideCanvasStrategy();
var hideLayerCmd = new ApplyStrategyCommand(hideLayerStrategy, hiddenLayer);
cmdManager.Execute(hideLayerCmd);

Console.WriteLine("\nStan po ukryciu warstwy:");
hiddenLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję UNDO:");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO:");
hiddenLayer.ConsoleDisplay();

Console.WriteLine("\nWykonuję REDO:");
cmdManager.Redo();

Console.WriteLine("Stan po REDO:");
hiddenLayer.ConsoleDisplay();

Console.WriteLine("\n>>> KONIEC TESTÓW UKRYWANIA/POKAZYWANIA <<<");

*/

// --- TEST 23: MÓJ TEST PRZESUWANIE
/*
Console.WriteLine("\n>>> TEST 23: PRZESUWANIE ELEMENTÓW <<<");

var cmdManager = new CommandManager();
var moveLayer = new Layer("Move Test Layer");
var rectToMove = new Rectangle(new Point(0, 0), new Point(10, 10), Color.FromRgb(1, 2, 3), Color.FromRgb(4, 5, 6), 1);
var blockedRect = new Rectangle(new Point(100, 100), new Point(110, 110), Color.FromRgb(7, 8, 9), Color.FromRgb(4, 5, 6), 1)
{
    IsBlocked = true // Ten nie powinien się ruszyć
};

moveLayer.Add(rectToMove);
moveLayer.Add(blockedRect);

Console.WriteLine("Stan początkowy:");
moveLayer.ConsoleDisplay();

// 1. Przesuwamy całą warstwę o (5, 10)
var moveStrategy = new MoveCanvasStrategy(5, 10);
var moveCmd = new ApplyStrategyCommand(moveStrategy, moveLayer);
    
Console.WriteLine("\nPrzesuwam warstwę o dx=5, dy=10...");
cmdManager.Execute(moveCmd);

Console.WriteLine("Stan po przesunięciu (niebieski powinien się zmienić, czerwony zostać w miejscu):");
moveLayer.ConsoleDisplay();

// 2. Test UNDO
Console.WriteLine("\nWykonuję UNDO...");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO (niebieski powinien wrócić na (0,0)):");
moveLayer.ConsoleDisplay();

// 3. Test przesuwania zaznaczenia (Selection)
Console.WriteLine("\n>>> TEST 24: PRZESUWANIE ZAZNACZENIA <<<");
// Tworzymy nową komendę przesunięcia tylko dla niebieskiego prostokąta (jako lista)
var selectMoveCmd = new ApplyStrategyCommand(new MoveCanvasStrategy(50, 50), new List<ICanvas> { rectToMove });
    
cmdManager.Execute(selectMoveCmd);
Console.WriteLine("Po przesunięciu samego niebieskiego o (50, 50):");
moveLayer.ConsoleDisplay();*/

// --- TEST 25: BLOKADA DODAWANIA DO ZABLOKOWANEJ WARSTWY ---
/*
Console.WriteLine("\n>>> TEST 25: BLOKADA DODAWANIA DO ZABLOKOWANEJ WARSTWY <<<");

var cmdManager = new CommandManager();
var lockedLayer = new Layer("Locked Layer")
{
    IsBlocked = true // Ręcznie blokujemy warstwę
};

var circleBuilder = new VectorEditor.Core.Builder.CircleBuilder("yellow", "black", 2)
    .SetStart(new Point(50, 50))
    .SetRadius(10);

var addCmd = new AddShapeCommand(circleBuilder, lockedLayer);

Console.WriteLine($"Próba dodania koła do warstwy '{lockedLayer.Name}' (IsBlocked: {lockedLayer.IsBlocked})...");
cmdManager.Execute(addCmd);

Console.WriteLine("\nStan warstwy po próbie dodania (powinna być pusta):");
lockedLayer.ConsoleDisplay();

// Sprawdźmy co się stanie przy Undo (nie powinno być błędów)
Console.WriteLine("\nWykonuję UNDO (cofam próbę dodania):");
cmdManager.Undo();
    
Console.WriteLine("Stan po UNDO:");
lockedLayer.ConsoleDisplay();

// --- TEST 26: DODAWANIE PO ODBLOKOWANIU ---
Console.WriteLine("\n>>> TEST 26: DODAWANIE PO ODBLOKOWANIU <<<");
    
var unblockStrategy = new UnblockCanvasStrategy();
cmdManager.Execute(new ApplyStrategyCommand(unblockStrategy, lockedLayer));
    
Console.WriteLine($"Warstwa odblokowana (IsBlocked: {lockedLayer.IsBlocked}). Ponowna próba dodania...");
    
var addSuccessCmd = new AddShapeCommand(circleBuilder, lockedLayer);
cmdManager.Execute(addSuccessCmd);

Console.WriteLine("\nStan warstwy po odblokowaniu i dodaniu:");
lockedLayer.ConsoleDisplay();*/

// --- TEST 27: PRZESKALOWANIE PROSTOKĄTA LUB LINII---
/*
Console.WriteLine("\n>>> TEST 27: PRZESKALOWANIE PROSTOKĄTA <<<");

var cmdManager = new CommandManager();
var scaleLayer = new Layer("Scale Test Layer");
var rectToScale = new Rectangle(new Point(10, 10), new Point(30, 30),  Colors.Green,  Colors.Black, 2);

scaleLayer.Add(rectToScale);

Console.WriteLine("Stan początkowy:");
scaleLayer.ConsoleDisplay();

// Przeskalowujemy prostokąt - przesuwamy prawy dolny róg do (50, 50)
var scaleStrategy = new ScaleStrategy(ScaleHandle.BottomRight, new Point(50, 50));
var scaleCmd = new ApplyStrategyCommand(scaleStrategy, rectToScale);

Console.WriteLine("\nPrzeskaluję prostokąt (przesuwam BottomRight do (50, 50))...");
cmdManager.Execute(scaleCmd);

Console.WriteLine("Stan po przeskalowaniu:");
scaleLayer.ConsoleDisplay();

// Test UNDO
Console.WriteLine("\nWykonuję UNDO...");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO (prostokąt powinien wrócić do (10,10)-(30,30)):");
scaleLayer.ConsoleDisplay();

// Test REDO
Console.WriteLine("\nWykonuję REDO...");
cmdManager.Redo();

Console.WriteLine("Stan po REDO (prostokąt powinien być znowu przeskalowany):");
scaleLayer.ConsoleDisplay();

Console.WriteLine("\n>>> KONIEC TESTU PRZESKALOWANIA <<<");
*/

// --- TEST 28: PRZESKALOWANIE TRÓJKĄTA Z KRAWĘDZIAMI RÓWNOLEGŁYMI DO OSI ---
/*
Console.WriteLine("\n>>> TEST 28: PRZESKALOWANIE TRÓJKĄTA <<<");

var cmdManager = new CommandManager();
var triangleLayer = new Layer("Triangle Scale Test Layer");
// Trójkąt prostokątny: dwie krawędzie równoległe do osi
// Krawędź pozioma: (10, 30) do (40, 30)
// Krawędź pionowa: (10, 10) do (10, 30)
// Przekątna: (10, 10) do (40, 30)
var triangleToScale = new Triangle(new Point(10, 10), new Point(40, 30), new Point(10, 30), Colors.Purple, Colors.Black, 2);

triangleLayer.Add(triangleToScale);

Console.WriteLine("Stan początkowy:");
triangleLayer.ConsoleDisplay();

// Przeskalowujemy trójkąt - przesuwamy prawy dolny róg do (60, 50)
var scaleTriangleStrategy = new ScaleStrategy(ScaleHandle.BottomRight, new Point(60, 50));
var scaleTriangleCmd = new ApplyStrategyCommand(scaleTriangleStrategy, triangleToScale);

Console.WriteLine("\nPrzeskaluję trójkąt (przesuwam BottomRight do (60, 50))...");
cmdManager.Execute(scaleTriangleCmd);

Console.WriteLine("Stan po przeskalowaniu:");
triangleLayer.ConsoleDisplay();

// Test UNDO
Console.WriteLine("\nWykonuję UNDO...");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO (trójkąt powinien wrócić do pierwotnych wymiarów):");
triangleLayer.ConsoleDisplay();

// Test REDO
Console.WriteLine("\nWykonuję REDO...");
cmdManager.Redo();

Console.WriteLine("Stan po REDO (trójkąt powinien być znowu przeskalowany):");
triangleLayer.ConsoleDisplay();

Console.WriteLine("\n>>> KONIEC TESTU PRZESKALOWANIA TRÓJKĄTA <<<");
*/

// --- TEST 29: PRZESKALOWANIE CUSTOMSHAPE (TRAPEZ PROSTOKĄTNY) ---
/*
Console.WriteLine("\n>>> TEST 29: PRZESKALOWANIE CUSTOMSHAPE (TRAPEZ PROSTOKĄTNY) <<<");

var cmdManager = new CommandManager();
var customShapeLayer = new Layer("CustomShape Scale Test Layer");
// Trapez prostokątny: lewa krawędź pionowa, prawa skośna
// Punkty: (10, 10), (50, 10), (60, 50), (10, 50)
var trapezoidToScale = new CustomShape(
    new List<Point>
    {
        new Point(10, 10),
        new Point(50, 10),
        new Point(60, 50),
        new Point(10, 50)
    },
    Colors.Orange,
    Colors.Black,
    2
);

customShapeLayer.Add(trapezoidToScale);

Console.WriteLine("Stan początkowy:");
customShapeLayer.ConsoleDisplay();

// Przeskalowujemy trapez - przesuwamy prawy dolny róg do (80, 60)
var scaleTrapezoidStrategy = new ScaleStrategy(ScaleHandle.BottomRight, new Point(80, 60));
var scaleTrapezoidCmd = new ApplyStrategyCommand(scaleTrapezoidStrategy, trapezoidToScale);

Console.WriteLine("\nPrzeskaluję trapez (przesuwam BottomRight do (80, 60))...");
cmdManager.Execute(scaleTrapezoidCmd);

Console.WriteLine("Stan po przeskalowaniu:");
customShapeLayer.ConsoleDisplay();

// Test UNDO
Console.WriteLine("\nWykonuję UNDO...");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO (trapez powinien wrócić do pierwotnych wymiarów):");
customShapeLayer.ConsoleDisplay();

// Test REDO
Console.WriteLine("\nWykonuję REDO...");
cmdManager.Redo();

Console.WriteLine("Stan po REDO (trapez powinien być znowu przeskalowany):");
customShapeLayer.ConsoleDisplay();

Console.WriteLine("\n>>> KONIEC TESTU PRZESKALOWANIA CUSTOMSHAPE <<<");
*/


// --- TEST 30: PRZESKALOWANIE WARSTWY Z PROSTYMI WSPÓŁRZĘDNYMI ---
/*
Console.WriteLine("\n>>> TEST 30: PRZESKALOWANIE WARSTWY (PROSTE PUNKTY) <<<");

var cmdManager = new CommandManager();
var layerToScale = new Layer("Layer Scale Test");

// Prostokąt od (0,0) do (10,10)
var simpleRect = new Rectangle(new Point(0, 0), new Point(10, 10), Colors.Blue, Colors.Black, 1);

// Trójkąt z prostymi współrzędnymi: (0,20), (10,20), (5,30)
var simpleTriangle = new Triangle(
    new Point(0, 20),
    new Point(10, 20),
    new Point(5, 30),
    Colors.Red,
    Colors.Black,
    1
);

layerToScale.Add(simpleRect);
layerToScale.Add(simpleTriangle);

Console.WriteLine("Stan początkowy warstwy:");
layerToScale.ConsoleDisplay();

// Przeskalowujemy warstwę - przesuwamy prawy dolny róg do (20, 40)
var scaleLayerStrategy = new ScaleStrategy(ScaleHandle.BottomRight, new Point(20, 40));
var scaleLayerCmd = new ApplyStrategyCommand(scaleLayerStrategy, layerToScale);

Console.WriteLine("\nPrzeskaluję warstwę (przesuwam BottomRight do (20, 40))...");
cmdManager.Execute(scaleLayerCmd);

Console.WriteLine("Stan po przeskalowaniu:");
layerToScale.ConsoleDisplay();

// Test UNDO
Console.WriteLine("\nWykonuję UNDO...");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO (warstwa powinna wrócić do pierwotnych wymiarów):");
layerToScale.ConsoleDisplay();

// Test REDO
Console.WriteLine("\nWykonuję REDO...");
cmdManager.Redo();

Console.WriteLine("Stan po REDO (warstwa powinna być znowu przeskalowana):");
layerToScale.ConsoleDisplay();

Console.WriteLine("\n>>> KONIEC TESTU PRZESKALOWANIA WARSTWY <<<");
*/

// --- TEST 31: PRZESKALOWANIE GRUPY ---
/*
Console.WriteLine("\n>>> TEST 30: PRZESKALOWANIE WARSTWY (PROSTE PUNKTY) <<<");

var cmdManager = new CommandManager();
var layerToScale = new Layer("Layer Scale Test");

// Prostokąt od (0,0) do (10,10)
var simpleRect = new Rectangle(new Point(0, 0), new Point(10, 10), "blue", "black", 1);

// Trójkąt z prostymi współrzędnymi: (0,20), (10,20), (5,30)
var simpleTriangle = new Triangle(
    new Point(0, 20),
    new Point(10, 20),
    new Point(5, 30),
    "red",
    "black",
    1
);

layerToScale.Add(simpleRect);
layerToScale.Add(simpleTriangle);

Console.WriteLine("Stan początkowy warstwy:");
layerToScale.ConsoleDisplay();

Console.WriteLine("Grupowanie elementów w warstwie");
var groupCmd = new SelectionCommand(layerToScale, new Point(0, 0),  new Point(10, 30));
groupCmd.Execute();
groupCmd.DisplayResults();

// Przeskalowujemy warstwę - przesuwamy prawy dolny róg do (20, 40)
var scaleLayerStrategy = new ScaleStrategy(ScaleHandle.BottomRight, new Point(20, 40));
var scaleLayerCmd = new ApplyStrategyCommand(scaleLayerStrategy, groupCmd.FoundElements);

Console.WriteLine("\nPrzeskaluję warstwę (przesuwam BottomRight do (20, 40))...");
cmdManager.Execute(scaleLayerCmd);

Console.WriteLine("Stan po przeskalowaniu:");
layerToScale.ConsoleDisplay();

// Test UNDO
Console.WriteLine("\nWykonuję UNDO...");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO (warstwa powinna wrócić do pierwotnych wymiarów):");
layerToScale.ConsoleDisplay();

// Test REDO
Console.WriteLine("\nWykonuję REDO...");
cmdManager.Redo();

Console.WriteLine("Stan po REDO (warstwa powinna być znowu przeskalowana):");
layerToScale.ConsoleDisplay();

Console.WriteLine("\n>>> KONIEC TESTU PRZESKALOWANIA WARSTWY <<<");


*/

// --- TEST 32: PRZESKALOWANIE OKRĘGU (ZMIANA W ELIPSĘ) ---
/*
Console.WriteLine("\n>>> TEST 32: PRZESKALOWANIE OKRĘGU (CIRCLE -> ELLIPSE) <<<");

var cmdManager = new CommandManager();
var circleLayer = new Layer("Circle Test Layer");

// Tworzymy okrąg: środek (10, 10), promień 5. 
// Zasięg geometryczny: X od 5 do 15, Y od 5 do 15.
var circle = new Circle(new Point(10, 10), 5, "yellow", "black", 2);

circleLayer.Add(circle);

Console.WriteLine("Stan początkowy (Okrąg):");
circleLayer.ConsoleDisplay();

// Wybieramy obszar zawierający okrąg
var groupCmd = new SelectionCommand(circleLayer, new Point(0, 0), new Point(20, 20));
groupCmd.Execute();

// Strategia: Rozciągamy BottomRight do (30, 20)
// To skalowanie jest nieproporcjonalne: 
// Szerokość grupy zmieni się z 10 (15-5) na 25 (30-5) -> sx = 2.5
// Wysokość grupy zmieni się z 10 (15-5) na 15 (20-5) -> sy = 1.5
var scaleStrategy = new ScaleStrategy(ScaleHandle.BottomRight, new Point(30, 20));
var applyScaleCmd = new ApplyStrategyCommand(scaleStrategy, groupCmd.FoundElements);

Console.WriteLine("\nSkaluję okrąg nieproporcjonalnie (BottomRight do (30, 20))...");
cmdManager.Execute(applyScaleCmd);

Console.WriteLine("Stan po przeskalowaniu (Powinna być Elipsa):");
circleLayer.ConsoleDisplay();
// Spodziewany wynik: RX = 5 * 2.5 = 12.5, RY = 5 * 1.5 = 7.5

// Test UNDO
Console.WriteLine("\nWykonuję UNDO...");
cmdManager.Undo();

Console.WriteLine("Stan po UNDO (Powrót do idealnego koła, R=5):");
circleLayer.ConsoleDisplay();

// Test REDO
Console.WriteLine("\nWykonuję REDO...");
cmdManager.Redo();

Console.WriteLine("Stan po REDO (Ponownie elipsa):");
circleLayer.ConsoleDisplay();

Console.WriteLine("\n>>> KONIEC TESTU OKRĘGU <<<");*/

// --- TEST 33: ZMIANA KOLEJNOŚCI (Z-ORDER) ---
/*
Console.WriteLine("\n>>> TEST 33: ZMIANA KOLEJNOŚCI OBIEKTÓW (Z-ORDER) <<<");

var cmdManager = new CommandManager();
var scene = new Layer("Main Scene");

// Tworzymy 3 obiekty
var redRect = new Rectangle(new Point(0,0), new Point(10,10), "red", "black", 1);
var greenRect = new Rectangle(new Point(5,5), new Point(15,15), "green", "black", 1);
var blueRect = new Rectangle(new Point(10,10), new Point(20,20), "blue", "black", 1);

// Dodajemy je kolejno: Czerwony (spód), Zielony (środek), Niebieski (wierzch)
scene.Add(redRect);
scene.Add(greenRect);
scene.Add(blueRect);

Console.WriteLine("Kolejność początkowa (od spodu do wierzchu):");
scene.ConsoleDisplay();

// 1. Przesuwamy Czerwony na sam wierzch (BringToFront)
Console.WriteLine("\n--- Wykonuję: BringToFront (Czerwony na wierzch) ---");
var toFrontCmd = new ChangeOrderCommand(redRect, OrderAction.BringToFront);
cmdManager.Execute(toFrontCmd);
scene.ConsoleDisplay(); // Spodziewane: Green, Blue, Red

// 2. Przesuwamy Niebieski o jeden poziom niżej (SendBackward)
Console.WriteLine("\n--- Wykonuję: SendBackward (Niebieski o jeden w dół) ---");
var backwardCmd = new ChangeOrderCommand(blueRect, OrderAction.SendBackward);
cmdManager.Execute(backwardCmd);
scene.ConsoleDisplay(); // Spodziewane: Blue, Green, Red (bo blue był na środku i spadł na spód)

// 3. Test UNDO (Cofamy SendBackward)
Console.WriteLine("\n--- Wykonuję: UNDO (Niebieski wraca na środek) ---");
cmdManager.Undo();
scene.ConsoleDisplay(); // Spodziewane: Green, Blue, Red

// 4. Test UNDO (Cofamy BringToFront)
Console.WriteLine("\n--- Wykonuję: UNDO (Czerwony wraca na spód) ---");
cmdManager.Undo();
scene.ConsoleDisplay(); // Spodziewane: Red, Green, Blue

Console.WriteLine("\n>>> KONIEC TESTU KOLEJNOŚCI <<<");*/

// --- TEST 34: COPY-CUT-PASTE SYSTEM ---
/*
Console.WriteLine("\n>>> TEST 34: SYSTEM KOPIOWANIA, WYCINANIA I WKLEJANIA <<<");

var cmdManager = new CommandManager();
var sourceLayer = new Layer("Source Layer");
var destinationLayer = new Layer("Destination Layer");

// 1. Przygotowanie obiektów
var rect = new Rectangle(new Point(0, 0), new Point(10, 10), Color.FromRgb(255, 255, 255), Color.FromRgb(255, 255, 255), 1);
var circle = new Circle(new Point(50, 50), 10, Color.FromRgb(255, 255, 255), Color.FromRgb(255, 255, 255), 1);
sourceLayer.Add(rect);
sourceLayer.Add(circle);

Console.WriteLine("Stan początkowy Source Layer:");
sourceLayer.ConsoleDisplay();

// 2. Symulacja CUT (Wycinanie)
Console.WriteLine("\n--- Wykonuję CUT (Wycinam oba obiekty z Source Layer) ---");
// Zazwyczaj tu użyłbyś FoundElements z SelectionCommand
var elementsToCut = new List<ICanvas> { rect, circle };
var cutCmd = new CutCommand(elementsToCut);
cmdManager.Execute(cutCmd);

Console.WriteLine("Source Layer po CUT (powinien być pusty):");
sourceLayer.ConsoleDisplay();

// 3. Symulacja PASTE (Wklejanie do nowej warstwy)
Console.WriteLine("\n--- Wykonuję PASTE (Wklejam do Destination Layer) ---");
var pasteCmd = new PasteCommand(destinationLayer);
cmdManager.Execute(pasteCmd);

Console.WriteLine("Destination Layer po PASTE (powinny być klony przesunięte o 10,10):");
destinationLayer.ConsoleDisplay();

// 4. Test UNDO PASTE
Console.WriteLine("\n--- Wykonuję UNDO PASTE ---");
cmdManager.Undo();
Console.WriteLine("Destination Layer (powinien być pusty):");
destinationLayer.ConsoleDisplay();

// 5. Test UNDO CUT
Console.WriteLine("\n--- Wykonuję UNDO CUT (obiekty powinny wrócić do Source Layer) ---");
cmdManager.Undo();
Console.WriteLine("Source Layer po UNDO CUT:");
sourceLayer.ConsoleDisplay();

// 6. Test REDO CUT + REDO PASTE
Console.WriteLine("\n--- Wykonuję REDO CUT i REDO PASTE (powrót do stanu z wklejonymi obiektami) ---");
cmdManager.Redo(); // Ponowne wycięcie
cmdManager.Redo(); // Ponowne wklejenie
Console.WriteLine("Source Layer (pusty):");
sourceLayer.ConsoleDisplay();
Console.WriteLine("Destination Layer (z klonami):");
destinationLayer.ConsoleDisplay();

Console.WriteLine("\n>>> KONIEC TESTU COPY-CUT-PASTE <<<");*/


var rootLayer = new Layer("Root Canvas");
var builder1 = new LineBuilder()
    .SetStart(new Point(0, 0))
    .SetContourColor(Colors.Black)
    .SetOpacity(100)
    .SetWidth(3)
    .SetEnd(new Point(4, 0));
    
var builder2 = new LineBuilder()
    .SetStart(new Point(8, 0))
    .SetContourColor(Colors.Black)
    .SetOpacity(100)
    .SetWidth(3)
    .SetEnd(new Point(12, 0));
    
var cmdManager = new CommandManager(new EditorContext());

cmdManager.Execute(new AddShapeCommand(builder1, rootLayer));
cmdManager.Execute(new AddShapeCommand(builder2, rootLayer));

var scale = new ScaleStrategy(ScaleHandle.Right, new Point(24, 0));
var scaleLayerCmd = new ApplyStrategyCommand(scale, rootLayer);
cmdManager.Execute(scaleLayerCmd);

rootLayer.ConsoleDisplay();