using System.Runtime.CompilerServices;
using UnityEngine;

public class SimpleFood : Food
{
    [SerializeField] private string identifier;
    public override string GetIdentifier(){
        return identifier;
    }
}
