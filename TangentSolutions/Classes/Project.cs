using System;
using System.Collections.Generic;

namespace TangentSolutions
{
	public class Project
	{
		public int PK { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool IsBillable { get; set; }
		public bool IsActive { get; set; }
		public List<Task> Tasks { get; set; }
		public List<Resources> Resources { get; set; }
	}
}

