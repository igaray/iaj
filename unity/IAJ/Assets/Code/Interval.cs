using System;


public class Interval
{
	int start;
	int end;
	
	public Interval (int start, int end)
	{
		this.start = start;
		this.end = end;
	}
	
	public int Start {
		get {
			return this.start;
		}
	}

	public int End {
		get {
			return this.end;
		}
	}
	
	public Boolean contains(int point) {
		return start <= point && point <= end;
	}
	
	public int getEnd() {
		return end;
	}
	
}


