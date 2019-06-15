using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace RDCore
{
    public class CallMethod
    {
        #region CONSTRUCTOR

        /// <summary>
        /// Gets the calling method.
        /// </summary>
        public CallMethod()
            : this(null)
        {
        }

        /// <summary>
        /// Gets the calling method, ignoring calls from the specified type.
        /// </summary>
        /// <param name="ignoreType">All calls made from this type will be ignored.
        /// Use this when wrapping this class in another class. OK if null.</param>
        public CallMethod(Type ignoreType)
        {
            this.ignoreType = ignoreType;
            this.Initialize();
        }

        #endregion CONSTRUCTORS

        #region PROPERTY

        private Type ignoreType;
        /// <summary>
        /// Type that will be ignored.
        /// </summary>
        public Type IgnoreType
        {
            get
            {
                return this.ignoreType;
            }
        }

        private MethodBase method;
        /// <summary>
        /// Calling method.
        /// </summary>
        public MethodBase Method
        {
            get
            {
                return this.method;
            }
        }

        private string methodNameFull;
        /// <summary>
        /// Full name of this method, with namespace.
        /// </summary>
        public string MethodNameFull
        {
            get
            {
                return this.methodNameFull;
            }
        }

        private string methodName;
        /// <summary>
        /// Name of this method.
        /// </summary>
        public string MethodName
        {
            get
            {
                return this.methodName;
            }
        }

        private int lineNumber;
        /// <summary>
        /// Line number in the file that called the method.
        /// </summary>
        public int LineNumber
        {
            get
            {
                return this.lineNumber;
            }
        }

        /// <summary>
        /// Namespace containing the object containing this method.
        /// </summary>
        public string Namespace
        {
            get
            {
                return type == null ? null : this.Type.Namespace;
            }
        }

        private string returnName;
        /// <summary>
        /// Name of the return type.
        /// </summary>
        public string ReturnName
        {
            get
            {
                return this.returnName;
            }
        }

        private string text;
        /// <summary>
        /// Full method signature, file and line number.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
        }

        private string typeNameFull;
        /// <summary>
        /// Full name of the type that contains this method,
        /// including the namespace.
        /// </summary>
        public string TypeNameFull
        {
            get
            {
                return this.typeNameFull;
            }
        }

        private string typeName;
        /// <summary>
        /// Name of the type that contains this method,
        /// not including the namespace.
        /// </summary>
        public string TypeName
        {
            get
            {
                return this.typeName;
            }
        }

        private Type type;
        /// <summary>
        /// Type that contains this method.
        /// </summary>
        public Type Type
        {
            get
            {
                return this.type;
            }
        }

        #endregion PROPERTY

        /// <summary>
        /// Initializes the calling method information.
        /// </summary>
        private void Initialize()
        {
            //METHOD BASE
            MethodBase method = null;
            string ignoreName = this.ignoreType == null ? null : this.ignoreType.Name;

            //STACK TRACE
            StackFrame stackFrame = null;
            StackTrace stackTrace = new StackTrace(true);
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                StackFrame sf = stackTrace.GetFrame(i);
                method = sf.GetMethod();
                string typeName = method.ReflectedType.Name;
                if (String.Compare(typeName, ignoreName) == 0)
                {
                    stackFrame = sf;
                    break;
                }
            }

            //METHOD
            method = stackFrame.GetMethod();
            this.method = method;
            string methodString = method.ToString();

            //TYPE
            this.type = method.ReflectedType;
            this.typeName = this.type.Name;
            this.typeNameFull = this.type.FullName;

            //METHOD
            this.methodName = method.Name;
            this.methodNameFull = String.Concat(this.typeNameFull, ".", this.methodName);

            this.lineNumber = stackFrame.GetFileLineNumber();
        }

        /// <summary>
        /// Gets the full method signature, file and line number.
        /// </summary>
        public override string ToString()
        {
            return this.Text;
        }
    }
}
