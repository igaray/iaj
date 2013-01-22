using UnityEngine;
using System.Collections;

public abstract class Entidad : MonoBehaviour {

	public string descripcion;
	public string tipo_de_la_entidad;
	// El nombre de la entidad debe ser único entre todas las entidades
	public string nombre; 
	public bool transportable;
	public Vector3 posicion;
	
	private string[] tipo_de_entidad = {"Agente", "Oro", "Taberna", "Poción"};
	
	public Entidad(string descripcion, string tipo_de_la_entidad, string nombre, bool transportable)
	{
		this.descripcion = descripcion;
		this.transportable = transportable;
		this.posicion = this.transform.position;
		this.nombre = nombre;
		if(checkTipoDeEntidadExistente(tipo_de_la_entidad))
		{
			this.tipo_de_la_entidad = tipo_de_la_entidad;
		} else {
			Debug.LogError("Clase Entidad: Llamada a constructor con tipo de entidad inválido <"+tipo_de_la_entidad+">");
		}
	}
	
	public string getDescripcion()
	{
		return this.descripcion;	
	}
	
	public string getTipoEntidad()
	{
		return this.tipo_de_la_entidad;
	}
	
	public string[] getTiposPosiblesDeEntidades()
	{
		return this.tipo_de_entidad;
	}
	
	public void setTipoEntidad(string tipo_de_la_entidad)
	{
		if(checkTipoDeEntidadExistente(tipo_de_la_entidad))
		{
			this.tipo_de_la_entidad = tipo_de_la_entidad;
		} else {
			Debug.LogError("Clase Entidad: Llamada a constructor con tipo de entidad inválido <"+tipo_de_la_entidad+">");
		}
	}
	
	public void addNuevoTipoDeEntidad(string nuevo_tipo_de_entidad)
	{
		if(!tipo_de_la_entidad.Equals(nuevo_tipo_de_entidad))
		{
			string[] nuevo_tipo_de_entidades = new string [this.tipo_de_entidad.Length+1];
			for(int i=0; i< this.tipo_de_entidad.Length; i++)
			{
				nuevo_tipo_de_entidades[i]=this.tipo_de_entidad[i];
			}
			nuevo_tipo_de_entidades[nuevo_tipo_de_entidades.Length-1]=nuevo_tipo_de_entidad;
			this.tipo_de_entidad = nuevo_tipo_de_entidades;
		}
	}
	
	public bool getTransportable()
	{
		return this.transportable;
	}
	
	public void setTransportable(bool transportable)
	{
		this.transportable = transportable;
	}
	
	public Vector3 getPosicion()
	{
		return this.posicion;
	}
	
	public void setPosicion(int x, int y, int z)
	{
		Vector3 nueva_posicion = new Vector3(x, y, z);
		this.posicion = nueva_posicion;
		this.transform.position = nueva_posicion;
	}
	
	public string getNombre()
	{
		return this.nombre;	
	}
	
	public void setNombre(string nombre)
	{
		this.nombre = nombre;	
	}
	
	private bool checkTipoDeEntidadExistente(string tipo_de_la_entidad)
	{
		for(int i=0; i<tipo_de_entidad.Length; i++) 
		{
			if(tipo_de_la_entidad.Equals(tipo_de_entidad[i]))
			{
				return true;
			}
		}
		return false;
	}
	
	public string getPercepcion() 
	{
		string percepcion = "";
		return percepcion;
	}
}
