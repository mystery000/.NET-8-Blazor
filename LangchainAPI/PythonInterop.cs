using Python.Runtime;

namespace LangchainAPI
{
    public class PythonInterop
    {
        public static void Initialize()
        {
            if (!PythonEngine.IsInitialized)
            {
                string pythonDll = @"C:\python\python310.dll";
                Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDll);
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();
            }

        }

        public static void RunPythonCode(string pycode)
        {
            Initialize();
            using (Py.GIL())
            {
                PythonEngine.RunSimpleString(pycode);
            }
        }

        public static void RunPythonCode(string pycode, object parameter, string parameterName)
        {
            Initialize();
            using (Py.GIL())
            {
                using (var scope = Py.CreateScope())
                {
                    scope.Set(parameterName, parameter.ToPython());
                    scope.Exec(pycode);
                }

            }
        }

        public static object RunPythonCodeAndReturn(string pycode, object parameter, string parameterName, string returnedVariableName)
        {
            object returnedVariable = new object();
            Initialize();
        
            using (Py.GIL())
            {
                using (var scope = Py.CreateScope())
                {
                    scope.Set(parameterName, parameter.ToPython());
                    scope.Exec(pycode);
                    returnedVariable = scope.Get<object>(returnedVariableName);
                }

            }
            return returnedVariable;
       
        }
    }
}
