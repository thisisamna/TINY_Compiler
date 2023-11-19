using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/*
 
 
 */
public enum Token_Class
{ //reserved words 
    INTEGER,FLOAT, STRING ,READ , 
    WRITE , REPEAT , UNTIL , IF , 
    ELSEIF , ELSE , THEN , RETURN ,ENDL,
    //Arithmetic operator 
    PlusOp, MinusOp, MultiplyOp, DivideOp,
    //Assignment operator :=
    AssignmentOp,
    Semicolon,
    //ConditionOperator (less than “<” | greater than “>” |is equal “=” | not equal “<>”)
    LessThanOp, GreaterThanOp, NotEqualOp, EqualOp,
    //Boolean_Operator: AND operator “&&” and OR operator “||”
    AndOp, OrOp,
    //
    Dot, LParanthesis, RParanthesis,Comma,
    //

    Idenifier, Number, String
}








namespace TINY_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            /*
            Reserved_Keywords: int | float | string | read | write | repeat | until |
            if | elseif | else | then | return | endl
             */
            ReservedWords.Add("int", Token_Class.INTEGER);
            ReservedWords.Add("float", Token_Class.FLOAT);
            ReservedWords.Add("string", Token_Class.STRING);
            ReservedWords.Add("read", Token_Class.READ);
            ReservedWords.Add("write", Token_Class.WRITE);
            ReservedWords.Add("repeat", Token_Class.REPEAT);
            ReservedWords.Add("until", Token_Class.UNTIL);
            ReservedWords.Add("if", Token_Class.IF);
            ReservedWords.Add("elseif", Token_Class.ELSEIF);
            ReservedWords.Add("else", Token_Class.ELSE);
            ReservedWords.Add("then", Token_Class.THEN);
            ReservedWords.Add("return", Token_Class.RETURN);
            ReservedWords.Add("endl", Token_Class.ENDL);
            /*
             arithmetic operation(+ | - | * | / )
             */
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            /*
             *Assignment_Statement: starts with Identifier then assignment operator “:=” 
             *followed by Expression (e.g. x := 1 | y:= 2+3 | z := 2+3*2+(2-3)/1 | …)
             */
            Operators.Add(":=", Token_Class.AssignmentOp);
            Operators.Add(";", Token_Class.Semicolon);
            /*
             *ConditionOperator (less than “<” | greater than “>” |
             *is equal “=” | not equal “<>”)
             */
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            //Boolean_Operator: AND operator “&&” and OR operator “||”
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
            //
            Operators.Add(".", Token_Class.Dot);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LParanthesis);
            Operators.Add("}", Token_Class.RParanthesis);
        }

        public void StartScanning(string SourceCode)
        {
            //Ignore the comments 
            string pattern_comment = @"/\*(.*?)\*/";
            SourceCode = Regex.Replace(SourceCode, pattern_comment, string.Empty);
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if ((CurrentChar >= 'A' && CurrentChar <= 'Z') || (CurrentChar >= 'a' && CurrentChar <= 'z')) //if you read a character
                {
                    j++;
                    if (j < SourceCode.Length)  //write "Iteration number [";
                    {
                        CurrentChar = SourceCode[j];
                        while (char.IsLetter(CurrentChar) || char.IsDigit(CurrentChar) )
                        {
                            CurrentLexeme += CurrentChar.ToString();
                            j++;
                            if (j >= SourceCode.Length)
                                break;
                            CurrentChar = SourceCode[j];
                        }
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j-1;
                }
  
                else if ((CurrentChar >= '0' && CurrentChar <= '9')) 
                {
                    j++;
                    if (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        while (char.IsDigit(CurrentChar)|| CurrentChar== '.' || char.IsLetter(CurrentChar))
                        {
                            CurrentLexeme += CurrentChar.ToString();
                            j++;
                            if (j >= SourceCode.Length)
                                break;
                            CurrentChar = SourceCode[j];
                        }
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j-1;
                }
                else if ((CurrentChar =='\"'))
                {
                    j++;
                    if (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        while (true)
                        {
                            CurrentLexeme += CurrentChar.ToString();
                            if (CurrentChar == '\"')
                                break;
                            j++;
                            if (j >= SourceCode.Length)
                                break;
                            CurrentChar = SourceCode[j];

                        }
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                else if(CurrentChar == ':')
                {
                    if (SourceCode[i+1]== '=') 
                    {
                        CurrentLexeme += SourceCode[i + 1];
                    }
                    FindTokenClass(CurrentLexeme);
                    i = i + 1;
                }
                
                else
                {
                    FindTokenClass(CurrentLexeme);
                }

                TINY_Compiler.TokenStream = Tokens;
            }
            void FindTokenClass(string Lex)
            {
                Token_Class TC;
                Token Tok = new Token();
                Tok.lex = Lex;
                //Is it a reserved word?
                if (ReservedWords.ContainsKey(Lex))
                {
                    TC = ReservedWords[Lex];
                    Tok.token_type = TC;
                    Tokens.Add(Tok);
                }
                //Is it an operator?
                else if (Operators.ContainsKey(Lex))
                {
                    TC = Operators[Lex];
                    Tok.token_type = TC;
                    Tokens.Add(Tok);
                }
                //Is it an identifier?
                else if (isIdentifier(Lex))
                {
                    TC = Token_Class.Idenifier;
                    Tok.token_type = TC;
                    Tokens.Add(Tok);
                }
                //Is it a Number?
                else if (isNumber(Lex))
                {
                    TC = Token_Class.Number;
                    Tok.token_type = TC;
                    Tokens.Add(Tok);
                }
                //Is it a String?
                else if (isString(Lex))
                {
                    TC = Token_Class.String;
                    Tok.token_type = TC;
                    Tokens.Add(Tok);
                }
                //Is it an undefined?
                else
                {
                    Errors.Error_List.Add(Lex);
                }
            }



            bool isIdentifier(string lex)
            {
                bool isValid = true;
                // Check if the lex is an identifier or not.
                //
                var pattern = new Regex("^[a-zA-Z]([a-zA-Z0-9])*$");
                if (!pattern.IsMatch(lex))
                {
                    isValid = false;
                }

                return isValid;
            }

            bool isString(string lex)
            {
                bool isValid = true;
                // Check if the lex is an identifier or not.
                //
                var pattern = new Regex("^\".*\"$");
                if (!pattern.IsMatch(lex))
                {
                    isValid = false;
                }
                return isValid;
            }

            bool isNumber(string lex)
            {
                bool isValid = true;
                // Check if the lex is a Number (Number) or not.
                var pattern = new Regex("^[0-9]+(.[0-9]+)?$");
                if (!pattern.IsMatch(lex))
                {
                    isValid = false;
                }
                return isValid;
            }
        }
    }
}