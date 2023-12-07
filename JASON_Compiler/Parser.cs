using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TINY_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            //Must be edited must find main in our code or not ?!
            root = new Node("Main_Function");
            root.Children.Add(Main_Function());
            return root;
        }

        private Node Main_Function()
        {
            Node Main_Function_var = new Node("Main_Function");
            Main_Function_var.Children.Add(Datatype());
            Main_Function_var.Children.Add(match(Token_Class.Main));
            Main_Function_var.Children.Add(match(Token_Class.LParanthesis));
            Main_Function_var.Children.Add(match(Token_Class.RParanthesis));
            Main_Function_var.Children.Add(Function_Body());
            return Main_Function_var;

        }
        private Node Cond()
        {
            //TODO: Task 4
            throw new NotImplementedException();
        }


        private Node Program()
        {
            Node Program_var = new Node("Program");
            Program_var.Children.Add(Fun_stmts());
            Program_var.Children.Add(Main_Function());
            return Program_var;
        }

        private Node Fun_stmts()
        {
            Node Fun_stmts_var = new Node("Fun_stmts");
            if (TokenStream[InputPointer].token_type == Token_Class.INTEGER|| TokenStream[InputPointer].token_type == Token_Class.FLOAT|| TokenStream[InputPointer].token_type == Token_Class.STRING)
            {
                Fun_stmts_var.Children.Add(Function_Statement());
                Fun_stmts_var.Children.Add(Fun_stmts());

            }
            else
            {
                return null;

            }
            return Fun_stmts_var;
        }
        private Node Function_Statement()
        {
            //TODO: Task 4
            throw new NotImplementedException();
        }

        private Node Datatype()
        {
            Node Datatype_var = new Node("Datatype");
            if (TokenStream[InputPointer].token_type == Token_Class.INTEGER)
            {
                Datatype_var.Children.Add(match(Token_Class.INTEGER));
                
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.FLOAT)
            {
                Datatype_var.Children.Add(match(Token_Class.FLOAT));

            }
            else
            {
                Datatype_var.Children.Add(match(Token_Class.STRING));

            }
            return Datatype_var;

        }

        private Node Function_Body()
        {
            //TODO: Task 4
            return null;
        }

        private Node Statements()
        {
            //i'm not really sure
            //Statements → ε Statements’

            Node statements = new Node("statements");
            statements.Children.Add(Statements2());
            return statements;

        }

        private Node Statements2()
        {
            //Statements’ → Statement Statements’ | ε
            Node statements2 = new Node("statements2");
            statements2.Children.Add(Statement());
            Statements2(); //recursion
            return statements2;

        }

        private Node Statement()
        {
            //me
            return null;

        }

        private Node Experssion()
        {
            throw new NotImplementedException();
        }
        private Node Cond_Stmt()
        {
            //Cond_Stmt -> cond cond_stmt2
            Node cond_stmt = new Node("Cond_Stmt");
            cond_stmt.Children.Add(Cond());
            cond_stmt.Children.Add(Cond_Stmt2());
            return cond_stmt;
        }

        private Node Cond_Stmt2()
        {
            //Cond_Stmt2 -> Bool_Cond Cond_Stmt2 | e
            Node cond_stmt2 = new Node("cond_stmt2");
            if (TokenStream[InputPointer].token_type==Token_Class.AndOp || TokenStream[InputPointer].token_type == Token_Class.OrOp)
            {
                cond_stmt2.Children.Add(Bool_Cond());
                cond_stmt2.Children.Add(Cond_Stmt2());
            }
            else
            {
                return null;
            }
            return cond_stmt2;
        }

        private Node Bool_Cond()
        {
            //Bool_Cond -> Bool_Op Cond
            Node bool_Cond = new Node("Bool_Cond");
            bool_Cond.Children.Add(Bool_Op());
            bool_Cond.Children.Add(Cond());
            return bool_Cond;
        }

        private Node Bool_Op()
        {
            //Bool_Op -> && | "||"
            Node bool_Op = new Node("Bool_Op");
            bool_Op.Children.Add(match(Token_Class.AndOp));
            bool_Op.Children.Add(match(Token_Class.OrOp));
            return bool_Op;
        }
        private Node Rep_Stmt()
        {
            //Rep_Stmt  -> “repeat”  Statements “until”  Cond_Stmt
            Node rep_Stmt = new Node("Rep_Stmt");
            rep_Stmt.Children.Add(match(Token_Class.REPEAT));
            rep_Stmt.Children.Add(Statements());
            rep_Stmt.Children.Add(match(Token_Class.UNTIL));
            rep_Stmt.Children.Add(Cond_Stmt());
            return rep_Stmt;
        }
        private Node Function_Call()
        {
            //Fun_call -> Identifier “(“  args  “)”
            Node function_Call = new Node("Function_Call");
            function_Call.Children.Add(match(Token_Class.Identifier));
            function_Call.Children.Add(match(Token_Class.LCurlyBrace));
            function_Call.Children.Add(args());
            function_Call.Children.Add(match(Token_Class.RCurlyBrace));
            return function_Call;
        }

        private Node args()
        {
            //args -> Identifier  args2  |   𝜀
            Node args_var = new Node("args");
            if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                args_var.Children.Add(match(Token_Class.Identifier));
                args_var.Children.Add(arg2());
            }
            else
            {
                return null;
            }
            return args_var;
        }


        private Node Dcl_stmt()//Declaration_Statement
        {
            Node Dcl_stmt_var = new Node("Dcl_stmt");
            Dcl_stmt_var.Children.Add(Datatype());
            Dcl_stmt_var.Children.Add(Identifiers());
            Dcl_stmt_var.Children.Add(match(Token_Class.Semicolon));
            return Dcl_stmt_var;

        }

        private Node Identifiers()//Declaration_Statement non_terminal
        {
            Node Identifiers_var = new Node("Identifiers");
            Identifiers_var.Children.Add(match(Token_Class.Identifier));
            if (TokenStream[InputPointer].token_type == Token_Class.AssignmentOp)
            {
                Identifiers_var.Children.Add(match(Token_Class.AssignmentOp));
                Identifiers_var.Children.Add(Iden2());

            }
            else
            {
                Identifiers_var.Children.Add(Iden2());

            }

            return Identifiers_var;

        }


        private Node Iden2()//Datatype
        {
            Node Iden2_var = new Node("Iden2");

            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                Iden2_var.Children.Add(match(Token_Class.Comma));
                Iden2_var.Children.Add(Identifiers());

            }
            else
            {
                return null;

            }
            return Iden2_var;

        }

        //private Node Assignment_Statement()//Assignment_Statement 
        //{
        //    return null;


        //}

        private Node arg2()
        {
            //args2 ->   “,”  Identifier  args2  |   𝜀
            Node args2_var = new Node("args2");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                args2_var.Children.Add(match(Token_Class.Comma));
                args2_var.Children.Add(match(Token_Class.Identifier));
                args2_var.Children.Add(arg2());
            }
            else
            {
                return null;
            }
            return args2_var;
        }
        private Node Ass_Stmt()
        {
            //ass_stmt -> Identifier  “:=”  experssion
            Node ass_stmt = new Node("Ass_Stmt");
            if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                ass_stmt.Children.Add(match(Token_Class.Identifier));
                ass_stmt.Children.Add(match(Token_Class.AssignmentOp));
                ass_stmt.Children.Add(Experssion());
            }
            return ass_stmt;
        }


        private Node Parameter()
        {
            //Parameter → Datatype Identifier
            Node parameter = new Node("parameter");

            parameter.Children.Add(DataType());
            if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                parameter.Children.Add(match(Token_Class.Identifier));
            }
            
            return parameter;
        }

        private Node Function_Statement()
        {
            //Function_Statement → Function_Decleration Function_Body
            Node function_statement = new Node("function_statement");

            function_statement.Children.Add(Function_Decleration());
            function_statement.Children.Add(Function_Body());
            return function_statement;
        }

        private Node Function_Decleration()
        {
            //Function_Declaration → Datatype Identifier (Parameters)

            Node function_declaration = new Node("function_declaration");

            function_declaration.Children.Add(DataType());
            if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                function_declaration.Children.Add(match(Token_Class.Identifier));
                function_declaration.Children.Add(match(Token_Class.LParanthesis));
                function_declaration.Children.Add(Parameters());
                function_declaration.Children.Add(match(Token_Class.RParanthesis));

            }
            function_declaration.Children.Add(Function_Body());
            return function_declaration;
        }

        private Node Parameters()
        {
            //Parameters → ε Parameters'
            Node parameters = new Node("parameters");
            parameters.Children.Add(Parameters2());
            return parameters;
        }

        private Node Parameters2()
        {
            //Parameters’ → Parameter Parameters’ | ε

            Node parameters2 = new Node("parameters2");

            parameters2.Children.Add(Parameter());
            Parameters2(); //recursion
            return parameters2; //?
        }

        private Node Function_Body()
        {
            //Function_Body → {Statements Retrun_Statment;}

            Node function_body = new Node("function_body");
            if (TokenStream[InputPointer].token_type == Token_Class.LCurlyBrace)
            {
                function_body.Children.Add(match(Token_Class.LCurlyBrace));
                function_body.Children.Add(Statements());
                function_body.Children.Add(Return_Statement());
                function_body.Children.Add(match(Token_Class.Semicolon));
                function_body.Children.Add(match(Token_Class.RCurlyBrace));
                return function_body; 
            }
            return null;
        }

        private Node DataType()
        {

            return null;
        }

        private Node Return_Statement()
        {

            return null;
        }




    }
}
