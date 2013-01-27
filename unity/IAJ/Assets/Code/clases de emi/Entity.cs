using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour {

	public string description;
	public string type;
	
	// El nombre de la entidad debe ser único entre todas las entidades
	public string entityName; 
	
	//public bool   moveable; //maybe needed, possibly not
	//public Vector3 position; //no need for position, since monobehaviour has transform
	
	//static public string[] tipo_de_entidad = {"agent", "gold", "inn", "potion"};
	
	public virtual Hashtable perception(){
		Hashtable p = new Hashtable();
		
		p["name"]        = name;
		p["type"]        = type;
		p["description"] = description;
		
		return p;
	}
	
	//son atributos publicos, entra cualquiera a toquetearlos
//	public string getDescripcion()
//	{
//		return this.descripcion;	
//	}
//	
//	public string getTipoEntidad()
//	{
//		return this.tipo_de_la_entidad;
//	}
//	
//	static public string[] getTiposPosiblesDeEntidades()
//	{
//		return this.tipo_de_entidad;
//	}
	
	//no creo que haya que agregar o cambiar tipos de entidad dinamicamente
//	public void setTipoEntidad(string tipo_de_la_entidad)
//	{
//		if(checkTipoDeEntidadExistente(tipo_de_la_entidad))
//		{
//			this.tipo_de_la_entidad = tipo_de_la_entidad;
//		} else {
//			Debug.LogError("Clase Entidad: Llamada a constructor con tipo de entidad inválido <"+tipo_de_la_entidad+">");
//		}
//	}
//	
//	public void addNuevoTipoDeEntidad(string nuevo_tipo_de_entidad)
//	{
//		if(!tipo_de_la_entidad.Equals(nuevo_tipo_de_entidad))
//		{
//			string[] nuevo_tipo_de_entidades = new string [this.tipo_de_entidad.Length+1];
//			for(int i=0; i< this.tipo_de_entidad.Length; i++)
//			{
//				nuevo_tipo_de_entidades[i]=this.tipo_de_entidad[i];
//			}
//			nuevo_tipo_de_entidades[nuevo_tipo_de_entidades.Length-1]=nuevo_tipo_de_entidad;
//			this.tipo_de_entidad = nuevo_tipo_de_entidades;
//		}
//	}
//	
//	public bool getTransportable()
//	{
//		return this.transportable;
//	}
//	
//	public void setTransportable(bool transportable)
//	{
//		this.transportable = transportable;
//	}
	
//	public Vector3 getPosicion()
//	{
//		return this.posicion;
//	}
//	
//	public void setPosicion(int x, int y, int z)
//	{
//		Vector3 nueva_posicion = new Vector3(x, y, z);
//		this.posicion = nueva_posicion;
//		this.transform.position = nueva_posicion;
//	}
	
//	public string getNombre()
//	{
//		return this.nombre;	
//	}
//	
//	public void setNombre(string nombre)
//	{
//		this.nombre = nombre;	
//	}
	
//	private bool checkTipoDeEntidadExistente(string tipo_de_la_entidad)
//	{
//		for(int i=0; i<tipo_de_entidad.Length; i++) 
//		{
//			if(tipo_de_la_entidad.Equals(tipo_de_entidad[i]))
//			{
//				return true;
//			}
//		}
//		return false;
//	}
	
//	public string getPercepcion() 
//	{
//		string percepcion = "";
//		return percepcion;
//	}
}
