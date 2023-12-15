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
            root.Children.Add(Program());
            return root;
        }

        private Node Main_Function()
        {
            //Main_Function → Datatype main() Function_Body

            Node Main_Function_var = new Node("Main_Function");
            Main_Function_var.Children.Add(Datatype());
            Main_Function_var.Children.Add(match(Token_Class.MAIN));
            Main_Function_var.Children.Add(match(Token_Class.LParanthesis));
            Main_Function_var.Children.Add(match(Token_Class.RParanthesis));
            Main_Function_var.Children.Add(Function_Body());
            return Main_Function_var;

        }
      
        private Node Condition() //condtion statement //cond()
        {
            //cond →  Identifier cond_op Term
            Node cond = new Node("Condition");

            cond.Children.Add(match(Token_Class.Identifier));
            cond.Children.Add(Condition_Operator());
            cond.Children.Add(Term());
            return cond;
               
        }
        
        private Node Condition_Operator() 
        {
            //cond_op →    < |   > |   = | <>
            Node condi = new Node("condition_Operator");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
                {
                    //>
                    condi.Children.Add(match(Token_Class.LessThanOp));
                    return condi;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
                {
                    //<
                    condi.Children.Add(match(Token_Class.GreaterThanOp));
                    return condi;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
                {
                    //=
                    condi.Children.Add(match(Token_Class.EqualOp));
                    return condi;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
                {
                    //<>
                    condi.Children.Add(match(Token_Class.NotEqualOp));
                    return condi;
                }
            }

            return condi;

        }
        private Node Term() //still not done yet
        {
            //Term → number | identifier | function_call
            Node term = new Node("Term");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Number)
                {
                    term.Children.Add(match(Token_Class.Number));
                    return term;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Identifier) //AMBUGUITY
                {
                    ++InputPointer; 
                    if (InputPointer < TokenStream.Count)
                    {
                        if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                        {
                            --InputPointer;
                            term.Children.Add(Function_Call());
                            return term;
                        }
                    }
                    
                    --InputPointer;
                    term.Children.Add(match(Token_Class.Identifier));
                    return term;

                }
                
            }
            return term;

        }
        private Node Read_Statement() //statement 
        {

            //Read_Statement → read Identifier ;

            Node read_Statement = new Node("Read_Statement");

            read_Statement.Children.Add(match(Token_Class.READ));
            read_Statement.Children.Add(match(Token_Class.Identifier));
            read_Statement.Children.Add(match(Token_Class.Semicolon));

            return read_Statement;
        }
        private Node Program() 
        {
            //Program → Fun_stmts Main_Function

            Node Program_var = new Node("Program");
            Program_var.Children.Add(Fun_stmts());
            Program_var.Children.Add(Main_Function());
            return Program_var;
        }

        private Node Fun_stmts()
        {
            //Fun_stmts → Function_Statement Fun_stmts | ε

            Node Fun_stmts_var = new Node("Fun_stmts");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.INTEGER || TokenStream[InputPointer].token_type == Token_Class.FLOAT || TokenStream[InputPointer].token_type == Token_Class.STRING)
                {
                    Fun_stmts_var.Children.Add(Function_Statement());
                    Fun_stmts_var.Children.Add(Fun_stmts());
                    return Fun_stmts_var;
                }
                else
                {
                    return null;

                }
            }
            return Fun_stmts_var;
        }

        private Node Datatype()
        {
            Node Datatype_var = new Node("Datatype");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.INTEGER)
                {
                    Datatype_var.Children.Add(match(Token_Class.INTEGER));
                    return Datatype_var;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.FLOAT)
                {
                    Datatype_var.Children.Add(match(Token_Class.FLOAT));
                    return Datatype_var;
                }
                else
                {
                    Datatype_var.Children.Add(match(Token_Class.STRING));
                    return Datatype_var;
                }
            }
            return Datatype_var;

        }



        private Node Statements() //statements
        {
            //Statements  → Statement Statements2 

            Node statements = new Node("statements");
            statements.Children.Add(Statement());
            statements.Children.Add(Statements2());
            return statements;

        }

        private Node Statements2()
        {
            //Statements2 →  Statements | ε 

            Node statements2 = new Node("statements2");
            if (InputPointer < TokenStream.Count)
            {
                bool isInteger = (TokenStream[InputPointer].token_type == Token_Class.INTEGER);
                bool isFloat = (TokenStream[InputPointer].token_type == Token_Class.FLOAT);
                bool isString = (TokenStream[InputPointer].token_type == Token_Class.String);
                bool isRead = (TokenStream[InputPointer].token_type == Token_Class.READ);
                bool isWrite = (TokenStream[InputPointer].token_type == Token_Class.WRITE);
                bool isRepeat = (TokenStream[InputPointer].token_type == Token_Class.REPEAT);
                bool isIf = (TokenStream[InputPointer].token_type == Token_Class.IF);
                bool isElseIf = (TokenStream[InputPointer].token_type == Token_Class.ELSEIF);
                bool isElse = (TokenStream[InputPointer].token_type == Token_Class.ELSE);
                bool isIdentifier = (TokenStream[InputPointer].token_type == Token_Class.Identifier);


                bool isStatement = isInteger || isFloat || isString || isRead || isWrite  ||
                                    isRepeat || isIf || isElseIf || isElse || isIdentifier;


                if (isStatement)
                {
                    statements2.Children.Add(Statements());
                    return statements2;
                }
                return null;
            }
            else
                return null; 

        }

        private Node Statement() //not done yet
        {
            Node statement = new Node("statement");
            if (InputPointer < TokenStream.Count)
            {
                bool isInteger = (TokenStream[InputPointer].token_type == Token_Class.INTEGER);
                bool isFloat = (TokenStream[InputPointer].token_type == Token_Class.FLOAT);
                bool isString = (TokenStream[InputPointer].token_type == Token_Class.String);

                if (isInteger || isFloat || isString)
                {
                    statement.Children.Add(Dcl_stmt());
                    return statement;
                }
                //AMBIGUITY
                if (TokenStream[InputPointer].token_type == Token_Class.Identifier) //Ass_Stmt() or Cond()
                {
                    ++InputPointer; //i dont know i just did what i thought was right
                    if (InputPointer < TokenStream.Count)
                    {
                        bool isLessOp = (TokenStream[InputPointer].token_type == Token_Class.LessThanOp);
                        bool isGreatOp = (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp);
                        bool isEqualOp = (TokenStream[InputPointer].token_type == Token_Class.EqualOp);
                        bool isNotEqualOp = (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp);
                        bool isAssignmentOp = (TokenStream[InputPointer].token_type == Token_Class.AssignmentOp);

                        
                        if (isAssignmentOp)
                        {
                            --InputPointer;
                            statement.Children.Add(Ass_Stmt());
                            return statement;
                        }
                        if (isLessOp || isGreatOp || isEqualOp || isNotEqualOp)
                        {
                            --InputPointer;
                            statement.Children.Add(Condition());
                            return statement;
                        }
                    }
                    --InputPointer;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.READ)
                {
                    statement.Children.Add(Read_Statement());
                    return statement;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.WRITE)
                {
                    statement.Children.Add(Write_Statement());
                    return statement;
                }

                if (TokenStream[InputPointer].token_type == Token_Class.REPEAT)
                {
                    statement.Children.Add(Rep_Stmt());
                    return statement;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.IF)
                {
                    statement.Children.Add(If_stat());
                    return statement;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
                {
                    statement.Children.Add(Else_if_stat());
                    return statement;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.ELSE)
                {
                    statement.Children.Add(Else_stat());
                    return statement;
                }
            }


            return statement ;

        }

        private Node Experssion() 
        {
            //Expression → string | Term | Equation
            Node expression = new Node("Expression");

            if (InputPointer < TokenStream.Count)
            {
                bool isString = (TokenStream[InputPointer].token_type == Token_Class.String);
                bool isNumber = (TokenStream[InputPointer].token_type == Token_Class.Number);
                bool isIdnetifier = (TokenStream[InputPointer].token_type == Token_Class.Identifier);

                
                if (isString)
                {
                    expression.Children.Add(match(Token_Class.String));
                    return expression;
                }
                //AMBIGUITY
                if (isNumber || isIdnetifier) //term or equation
                {
                    ++InputPointer; 
                    if (InputPointer < TokenStream.Count)
                    {
                        bool isPlusOp = (TokenStream[InputPointer].token_type == Token_Class.PlusOp);
                        bool isMinusOp = (TokenStream[InputPointer].token_type == Token_Class.MinusOp);
                        bool isMultiplyOp = (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp);
                        bool isDivideOp = (TokenStream[InputPointer].token_type == Token_Class.DivideOp);

                        
                        if (isPlusOp || isMinusOp || isMultiplyOp || isDivideOp)
                        {
                            --InputPointer;
                            expression.Children.Add(Equation());
                            return expression;
                        }
                    }
                    --InputPointer;
                    expression.Children.Add(Term()); 
                    return expression;
                }

            }
            return expression;



        }

        private Node Equation()
        {
            //Equation → Equation2 op Equation2

            Node equation = new Node("equation");
            equation.Children.Add(Equation2());
            equation.Children.Add(Arthmetic_Operator());
            equation.Children.Add(Equation2());
            return equation;
        }

        private Node Equation2()
        {
            //Equation2 → Term | (Equation)

            Node equation2 = new Node("equation2");
            if (InputPointer < TokenStream.Count)
            {
                bool isNumber = (TokenStream[InputPointer].token_type == Token_Class.Number);
                bool isIdnetifier = (TokenStream[InputPointer].token_type == Token_Class.Identifier);
                bool isParanthesis = (TokenStream[InputPointer].token_type == Token_Class.LParanthesis);


                if (isNumber || isIdnetifier) //if it is term
                {
                    equation2.Children.Add(Term());
                    return equation2;
                }
                else if (isParanthesis)
                {
                    equation2.Children.Add(match(Token_Class.LParanthesis));
                    equation2.Children.Add(Equation());
                    equation2.Children.Add(match(Token_Class.RParanthesis));
                    return equation2;
                }
            }

            return equation2;
        }

        private Node Arthmetic_Operator() //op
        {
            //op → + | - | * | /

            Node op = new Node("op");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                {
                    op.Children.Add(match(Token_Class.PlusOp));
                    return op;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                {
                    op.Children.Add(match(Token_Class.MinusOp));
                    return op;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                {
                    op.Children.Add(match(Token_Class.MultiplyOp));
                    return op;
                }
                if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                {
                    op.Children.Add(match(Token_Class.DivideOp));
                    return op;
                }

            }

            return op;
        }




        private Node Write_Statement() //statement 
        {
            //wt_stmt → Write W
            Node wt_stmt = new Node("Write_Statement");


            wt_stmt.Children.Add(match(Token_Class.WRITE));
            wt_stmt.Children.Add(W());
            return wt_stmt;

        }

        private Node W() 
        {
            //W→ Expression; | endl
            
            Node w = new Node("W");
  
                if (InputPointer < TokenStream.Count)
                {
                    bool isString = (TokenStream[InputPointer].token_type == Token_Class.String);
                    bool isNumber = (TokenStream[InputPointer].token_type == Token_Class.Number);
                    bool isIdnetifier = (TokenStream[InputPointer].token_type == Token_Class.Identifier);
                    if (isString || isNumber || isIdnetifier)
                    {
                        w.Children.Add(Experssion());
                        w.Children.Add(match(Token_Class.Semicolon));
                        return w;
                    }
                    else if (TokenStream[InputPointer].token_type == Token_Class.ENDL)
                    {
                        w.Children.Add(match(Token_Class.ENDL));
                        w.Children.Add(match(Token_Class.Semicolon));
                        return w;

                    }
                    
                }
            
            return w;
        }
        private Node Cond_Stmt()
        {
            //Cond_Stmt -> cond cond_stmt2
            Node cond_stmt = new Node("Cond_Stmt");
            cond_stmt.Children.Add(Condition());
            cond_stmt.Children.Add(Cond_Stmt2());
            return cond_stmt;
        }

        private Node Cond_Stmt2()
        {
            //Cond_Stmt2 -> Bool_Cond Cond_Stmt2 | e
            Node cond_stmt2 = new Node("cond_stmt2");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.AndOp || TokenStream[InputPointer].token_type == Token_Class.OrOp)
                {
                    cond_stmt2.Children.Add(Bool_Cond());
                    cond_stmt2.Children.Add(Cond_Stmt2());
                    return cond_stmt2;
                }
                else
                {
                    return null;
                }
            }
            return cond_stmt2;
        }

        private Node Bool_Cond()
        {
            //Bool_Cond -> Bool_Op Cond
            Node bool_Cond = new Node("Bool_Cond");
            bool_Cond.Children.Add(Bool_Op());
            bool_Cond.Children.Add(Condition());
            return bool_Cond;
        }

        private Node Bool_Op()
        {
            //Bool_Op -> && | "||"
            Node bool_Op = new Node("Bool_Op");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.AndOp)
                {
                    bool_Op.Children.Add(match(Token_Class.AndOp));
                    return bool_Op;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.OrOp)
                {
                    bool_Op.Children.Add(match(Token_Class.OrOp));
                    return bool_Op;
                }
                else
                {
                    //add to errors list?
                }
            }
                
            return bool_Op;
        }
        private Node Rep_Stmt() //statement
        {
            //Rep_Stmt  -> repeat  Statements until  Cond_Stmt
            Node rep_Stmt = new Node("Rep_Stmt");
            rep_Stmt.Children.Add(match(Token_Class.REPEAT));
            rep_Stmt.Children.Add(Statements());
            rep_Stmt.Children.Add(match(Token_Class.UNTIL));
            rep_Stmt.Children.Add(Cond_Stmt());
            return rep_Stmt;
        }
        private Node Function_Call()
        {
            //Fun_call -> Identifier (  args  )
            Node function_Call = new Node("Function_Call");
            function_Call.Children.Add(match(Token_Class.Identifier));
            function_Call.Children.Add(match(Token_Class.LParanthesis));
            function_Call.Children.Add(args());
            function_Call.Children.Add(match(Token_Class.RParanthesis));
            return function_Call;
        }

        private Node args()
        {
            //args -> Identifier  args2  |   𝜀
            Node args_var = new Node("args");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    args_var.Children.Add(match(Token_Class.Identifier));
                    args_var.Children.Add(arg2());
                    return args_var;
                }
                else
                {
                    return null;
                }
            }
            return args_var;
        }

        private Node arg2()
        {
            //args2 ->   “,”  Identifier  args2  |   𝜀
            Node args2_var = new Node("args2");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    args2_var.Children.Add(match(Token_Class.Comma));
                    args2_var.Children.Add(match(Token_Class.Identifier));
                    args2_var.Children.Add(arg2());
                    return args2_var;
                }
                else
                {
                    return null;
                }
            }
            return args2_var;
        }

        private Node Dcl_stmt()//Declaration_Statement
        {
            //Dcl_stmt →  Datatype Identifiers ;
            Node Dcl_stmt_var = new Node("Dcl_stmt");
            Dcl_stmt_var.Children.Add(Datatype());
            Dcl_stmt_var.Children.Add(Identifiers());
            Dcl_stmt_var.Children.Add(match(Token_Class.Semicolon));
            return Dcl_stmt_var;

        }

        private Node Identifiers()//Declaration_Statement non_terminal 
        {
            //Identifiers → Assignment_Statement Iden2 | Identifier Iden2
            //Identifiers → Iden3 Iden2
            //Iden3 → Identifier  | Assignment_Statement
            Node Identifiers_var = new Node("Identifiers");
            //Identifiers_var.Children.Add(match(Token_Class.Identifier));

            Identifiers_var.Children.Add(Iden3());
            //Identifiers_var.Children.Add(match(Token_Class.AssignmentOp));
            Identifiers_var.Children.Add(Iden2());
            return Identifiers_var;
   


        }


        private Node Iden2()//Datatype //not checked
        {

            //Iden2 → , Identifiers | ε
            Node Iden2_var = new Node("Iden2");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    Iden2_var.Children.Add(match(Token_Class.Comma));
                    Iden2_var.Children.Add(Identifiers());
                    return Iden2_var;
                }
                else
                {
                    return null;

                }
            }
            return Iden2_var;

        }

        private Node Iden3()
        {
            //Iden3 → Identifier  | Assignment_Statement
            Node Iden3 = new Node("Iden3");
            //Identifiers_var.Children.Add(match(Token_Class.Identifier));
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    ++InputPointer;
                    if (InputPointer < TokenStream.Count)
                    {
                        if (TokenStream[InputPointer].token_type == Token_Class.AssignmentOp)
                        {
                            --InputPointer;
                            Iden3.Children.Add(Ass_Stmt());
                            return Iden3;
                        }
                    }
                    --InputPointer;
                    Iden3.Children.Add(match(Token_Class.Identifier));
                    return Iden3;

                }
                
            }
            return null;

        }



        private Node Ass_Stmt() //assignment statement
        {
            //ass_stmt →  Identifier  :=  experssion
            Node ass_stmt = new Node("Ass_Stmt");
           
            ass_stmt.Children.Add(match(Token_Class.Identifier));
            ass_stmt.Children.Add(match(Token_Class.AssignmentOp));
            ass_stmt.Children.Add(Experssion());
          
            return ass_stmt;
        }


        private Node Parameter() 
        {
            //Parameter → Datatype Identifier
            Node parameter = new Node("parameter");

            parameter.Children.Add(Datatype());
            parameter.Children.Add(match(Token_Class.Identifier));
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

            function_declaration.Children.Add(Datatype());
            function_declaration.Children.Add(match(Token_Class.Identifier));
            function_declaration.Children.Add(match(Token_Class.LParanthesis));
            function_declaration.Children.Add(Parameters());
            function_declaration.Children.Add(match(Token_Class.RParanthesis));

            return function_declaration;
        }
        
        private Node Parameters() 
        {
            //Parameters → Parameter Parameters2 | ε
          
            Node parameters = new Node("parameters");

            if (InputPointer < TokenStream.Count)
            {
                bool isInteger = (TokenStream[InputPointer].token_type == Token_Class.INTEGER);
                bool isFloat = (TokenStream[InputPointer].token_type == Token_Class.FLOAT);
                bool isString = (TokenStream[InputPointer].token_type == Token_Class.String);

                if (isInteger || isFloat || isString)
                {
                    parameters.Children.Add(Parameter());
                    parameters.Children.Add(Parameters2());
                    return parameters;
                }
                else
                    return null;
            }
            return null;
        }

        private Node Parameters2()
        {
            //Parameters2 →  “,”  Parameters | ε

            Node parameters2 = new Node("parameters2");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    parameters2.Children.Add(match(Token_Class.Comma));
                    parameters2.Children.Add(Parameters());
                    return parameters2;
                }
                else
                    return null;
            }
            return null;

        }


        private Node Function_Body() 
        {
            //Function_Body → { Statements Ret_stmt }

            Node function_body = new Node("function_body");
            function_body.Children.Add(match(Token_Class.LBrace));
            function_body.Children.Add(Statements());
            function_body.Children.Add(Return_Statement());
            function_body.Children.Add(match(Token_Class.RBrace));
            return function_body;
 
        }

        private Node Return_Statement() 
        {
            //ret_stmt → return Expression ;

            Node ret = new Node("Return_Statment");

            ret.Children.Add(match(Token_Class.RETURN));
            ret.Children.Add(Experssion());
            ret.Children.Add(match(Token_Class.Semicolon));
            return ret;



        }

        private Node If_stat()  
        {
            //if_stat →  if Cond_Stmt then Statements else_if_stat else_stat end
            Node if_stat = new Node("if_stat");

            if_stat.Children.Add(match(Token_Class.IF));
            if_stat.Children.Add(Cond_Stmt());
            if_stat.Children.Add(match(Token_Class.THEN));
            if_stat.Children.Add(Statements());
            if_stat.Children.Add(Else_if_stat());
            if_stat.Children.Add(Else_stat());
            if_stat.Children.Add(match(Token_Class.ENDL));


            return if_stat;
        }

        private Node Else_if_stat() 
        {
            //else_if_stat  →  elseif Cond_Stmt then Statements  else_if_stat  else_stat  end 

            Node else_if_stat = new Node("else_if_stat");
            
            else_if_stat.Children.Add(match(Token_Class.ELSEIF));
            else_if_stat.Children.Add(Cond_Stmt());
            else_if_stat.Children.Add(match(Token_Class.THEN));
            else_if_stat.Children.Add(Statements());
            else_if_stat.Children.Add(Else_if_stat());
            else_if_stat.Children.Add(Else_stat());
            else_if_stat.Children.Add(match(Token_Class.ENDL));


            return else_if_stat;
                
        }

        private Node Else_stat() 
        {
            //else_stat → else Statements end   
            Node else_stat = new Node("else_stat");


            else_stat.Children.Add(match(Token_Class.ELSE));
            else_stat.Children.Add(Statements());
            else_stat.Children.Add(match(Token_Class.ENDL));

            return else_stat;
           
        }


        }
    } 


