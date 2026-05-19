using DiceRoller.Services;
using DiceRoller.Ui;

Console.Title = "Dice Roller";

var randomSource = new CryptoRandomSource();
var roller = new DiceRollerService(randomSource);
var parser = new DiceExpressionParser();
var history = new RollHistory();

var guidedPrompt = new GuidedRollPrompt(roller, history);
var expressionPrompt = new ExpressionEntryPrompt(parser, roller, history);
var rerollPrompt = new RerollHistoryPrompt(history, roller);

var menu = new MainMenu(guidedPrompt, expressionPrompt, rerollPrompt);
menu.Run();
