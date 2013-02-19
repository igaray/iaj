using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PrologList : IPerceivableEntity{
	
	public List<IPerceivableEntity> list;
	
	public PrologList(IList l){
		list = l.Cast<IPerceivableEntity>().ToList();
	}
	
	public string toProlog (){
		string aux = "[";
		if (list.Count==0)
			aux += "]";
		else{
			foreach(IPerceivableEntity e in list){
				aux += e.toProlog() + ",";
			}	
			aux = aux.TrimEnd(",".ToCharArray()) + "]";
		}
		return aux;
	}
	
//	public PrologList addPair(string key, string elem){
//		list.Add(new PrologList(new List<string>().Add(String.Format("[{0}, {1}]", key, elem))));
//	}
	
	static public string AtomList<T>(List<T> list){
			
		string aux = "[";
		if (list.Count==0)
			aux += "]";
		else{
			foreach(T e in list){
				aux += e.ToString() + ",";
			}	
			aux = aux.TrimEnd(",".ToCharArray()) + "]";
		}
		return aux;
	
	}
}
