using System.Text.RegularExpressions;

var pattern = @"^(?=.{1,90}$)(?:build|feat|ci|chore|docs|fix|perf|refactor|revert|style|test)(?:\(.+\))*(?::).{4,}(?:#\d+)*(?<![\.\s])$";
var msg = File.ReadAllLines(Args[0])[0];

if (Regex.IsMatch(msg, pattern))
   return 0;
   
Console.WriteLine("Invalid commit message");
Console.WriteLine("e.g: 'feat(scope): subject' or 'fix: subject'");

return 1;