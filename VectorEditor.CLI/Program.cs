using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;
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

var removeStrategy = new VectorEditor.Core.Strategy.RemoveStrategy();
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

var selectLegsCmd = new GroupCommand(bodyLayer, new Point(35, 115), new Point(65, 170));
cmdManager.Execute(selectLegsCmd);

Console.WriteLine("\nZnalezione elementy (nogi):");
selectLegsCmd.DisplayResults();

var removeGroupStrategy = new VectorEditor.Core.Strategy.RemoveStrategy();
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

var selectAllCmd = new GroupCommand(parentLayer, new Point(0, 0), new Point(200, 200));
cmdManager.Execute(selectAllCmd);

Console.WriteLine("\nZnalezione wszystkie elementy:");
selectAllCmd.DisplayResults();

var removeAllStrategy = new VectorEditor.Core.Strategy.RemoveStrategy();
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

var removeStrategy1 = new VectorEditor.Core.Strategy.RemoveStrategy();
var remove1 = new ApplyStrategyCommand(removeStrategy1, shape1);
var removeStrategy2 = new VectorEditor.Core.Strategy.RemoveStrategy();
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
var selectAllCmd = new GroupCommand(mixedLayer, new Point(90, 90), new Point(150, 110));
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
var rectToMove = new Rectangle(new Point(0, 0), new Point(10, 10), "blue", "black", 1);
var blockedRect = new Rectangle(new Point(100, 100), new Point(110, 110), "red", "black", 1)
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