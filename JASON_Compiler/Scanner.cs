﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Token_Class
{ //reserved words 
    INTEGER,FLOAT, STRING ,READ , 
    WRITE , REPEAT , UNTIL , IF , 
    ELSEIF , ELSE , THEN , RETURN ,END,
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
    Idenifier, Constant
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
            ReservedWords.Add("end", Token_Class.END);
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
            
           
            



        }

    public void StartScanning(string SourceCode)
        {
            for(int i=0; i<SourceCode.Length;i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {
                   
                }

                else if(CurrentChar >= '0' && CurrentChar <= '9')
                {
                   
                }
                else if(CurrentChar == '{')
                {
                   
                }
                else
                {
                   
                }
            }
            
            TINY_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            

            //Is it an identifier?
            

            //Is it a Constant?

            //Is it an operator?

            //Is it an undefined?
        }

    

        bool isIdentifier(string lex)
        {
            bool isValid=true;
            // Check if the lex is an identifier or not.
            
            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = true;
            // Check if the lex is a constant (Number) or not.

            return isValid;
        }
    }
}
