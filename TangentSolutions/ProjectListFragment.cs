
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace TangentSolutions
{
	[Activity(Label = "ProjectListActivity")]
	public class ProjectListFragment : Android.Support.V4.App.Fragment
	{
		static List<Project> projects;
		public ProjectListFragment(List<Project> _projects)
		{
			projects = _projects;
		}
		public static Android.Support.V4.App.Fragment newInstance(Context context)
		{
			ProjectListFragment busrouteFragment = new ProjectListFragment(projects);
			return busrouteFragment;
		}
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

		}
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Utils utils = new Utils();
			utils.pref = Application.Context.GetSharedPreferences("Session Data", FileCreationMode.Private);
			utils.edit = utils.pref.Edit();

			ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.ProjectList, null);

			ListView lvProjectList = root.FindViewById<ListView>(Resource.Id.lvProjectList);

			lvProjectList.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
			{
				utils.edit.PutBoolean("IsCreate", false).Commit();

				Project selectedProject = new Project() { Description = projects.ElementAt(e.Position).Description, EndDate = projects.ElementAt(e.Position).EndDate, IsActive = projects.ElementAt(e.Position).IsActive, IsBillable = projects.ElementAt(e.Position).IsBillable, PK = projects.ElementAt(e.Position).PK, Resources = null, StartDate = projects.ElementAt(e.Position).StartDate, Tasks = null, Title = projects.ElementAt(e.Position).Title };

				// Create a new fragment and a transaction.
				Android.Support.V4.App.FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
				ProjectDetailsFragment aDifferentDetailsFrag = new ProjectDetailsFragment(selectedProject);

				// Replace the fragment that is in the View fragment_container (if applicable).
				fragmentTx.Replace(Resource.Id.menu, aDifferentDetailsFrag);

				// Add the transaction to the back stack.
				fragmentTx.AddToBackStack(null);

				// Commit the transaction.
				fragmentTx.Commit();
			};

			List<string> projectTitle = new List<string>();

			foreach (Project project in projects)
				projectTitle.Add(project.Title);

			ArrayAdapter ListAdapter = new ArrayAdapter<String>(root.Context, Android.Resource.Layout.SimpleListItem1, projectTitle);
			lvProjectList.Adapter = ListAdapter;

			return root;
		}

	}
}

