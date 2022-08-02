using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singelton<T> where T : class, new()
{
    private static T ins;
    private static object obj=new object();
    
    public static T getsingelton() 
    {
        if (ins==null)
        {
            lock (obj)
            {
                if (ins==null)
                {
                    ins = new T();
                }
            }
        }
        return ins;
    }
}
