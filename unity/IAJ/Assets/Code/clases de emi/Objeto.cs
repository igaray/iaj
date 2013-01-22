using UnityEngine;
using System.Collections;

public class Objeto : Entidad {
	
	public int peso;
	private int max_cant_usos;
	public int cant_usos_restantes;
	
	public Objeto(int peso, int max_cant_usos, string descripcion, string tipo_objeto, string nombre) 
		: base(descripcion, tipo_objeto, nombre, true) 
	{
		this.peso = peso;
		this.max_cant_usos = max_cant_usos;
		this.cant_usos_restantes = max_cant_usos;
	}
	
	public int getPeso()
	{
		return this.peso;
	}
}
