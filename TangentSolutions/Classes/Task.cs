using System;
using System.Collections.Generic;

namespace TangentSolutions
{
	public class Task
	{
		public int PK { get; set; }
		public string Title { get; set; }
		public DateTime DueDate { get; set; }
		public double EstimatedHours { get; set; }
		public int Project { get; set; }
		public List<Data> ProjectData { get; set; }
	}
}

