
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportFragment = Android.Support.V4.App.Fragment;
using Newtonsoft.Json;

namespace TangentSolutions
{
	[Activity(Label = "MenuActivity")]
	public class MenuActivity : ActionBarActivity
	{
		private SupportToolbar mToolbar;
		private MyActionBarDrawerToggle mDrawerToggle;
		private DrawerLayout mDrawerLayout;
		private ListView mLeftDrawer;
		private ProjectListFragment homeFragment;
		private ProjectDetailsFragment profileFragment;
		private SupportFragment mCurrentFragment = new SupportFragment();
		private Stack<SupportFragment> mStackFragments;

		private ArrayAdapter mLeftAdapter;

		private List<string> mLeftDataSet;
		List<Project> MyObject;
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Menu);

			var MyJsonString = Intent.GetStringExtra("MyProjects");
			MyObject = JsonConvert.DeserializeObject<List<Project>>(MyJsonString);

			mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
			mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
			homeFragment = new ProjectListFragment(MyObject);

			profileFragment = new ProjectDetailsFragment(null);
			mStackFragments = new Stack<SupportFragment>();


			mLeftDrawer.Tag = 0;

			SetSupportActionBar(mToolbar);

			mLeftDataSet = new List<string>();
			mLeftDataSet.Add("Project List");
			mLeftDataSet.Add("Create Project");
			mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
			mLeftDrawer.Adapter = mLeftAdapter;
			mLeftDrawer.ItemClick += MenuListView_ItemClick;


			mDrawerToggle = new MyActionBarDrawerToggle(
				this,                           //Host Activity
				mDrawerLayout,                  //DrawerLayout
				Resource.String.openDrawer,     //Opened Message
				Resource.String.closeDrawer     //Closed Message
			);

			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			mDrawerLayout.SetDrawerListener(mDrawerToggle);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayShowTitleEnabled(true);
			mDrawerToggle.SyncState();

			SupportActionBar.Title = "Menu";

			Android.Support.V4.App.FragmentTransaction tx = SupportFragmentManager.BeginTransaction();

			tx.Add(Resource.Id.menu, homeFragment);
			tx.Add(Resource.Id.menu, profileFragment);
			tx.Hide(profileFragment);

			mCurrentFragment = homeFragment;
			tx.Commit();
		}
		void MenuListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			Android.Support.V4.App.Fragment fragment = null;


			Utils utils = new Utils();
			utils.pref = Application.Context.GetSharedPreferences("Session Data", FileCreationMode.Private);
			utils.edit = utils.pref.Edit();

			switch (e.Id)
			{
				case 0:
					utils.edit.PutBoolean("IsCreate", false).Commit();
					ShowFragment(homeFragment);
					break;
				case 1:
					utils.edit.PutBoolean("IsCreate", true).Commit();
					ShowFragment(profileFragment);
					break;



			}

			mDrawerLayout.CloseDrawers();
			mDrawerToggle.SyncState();

		}
		public void ShowFragment(SupportFragment fragment)
		{

			if (fragment.IsVisible)
			{
				return;
			}

			try
			{
				var trans = SupportFragmentManager.BeginTransaction();

				fragment.View.BringToFront();
				mCurrentFragment.View.BringToFront();

				trans.Hide(mCurrentFragment);
				trans.Show(fragment);

				trans.AddToBackStack(null);
				mStackFragments.Push(mCurrentFragment);
				trans.Commit();

				mCurrentFragment = fragment;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		}
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{

				case Android.Resource.Id.Home:
					//The hamburger icon was clicked which means the drawer toggle will handle the event

					mDrawerToggle.OnOptionsItemSelected(item);
					return true;

				default:
					return base.OnOptionsItemSelected(item);
			}
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			return base.OnCreateOptionsMenu(menu);
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
			{
				outState.PutString("DrawerState", "Opened");
			}

			else
			{
				outState.PutString("DrawerState", "Closed");
			}

			base.OnSaveInstanceState(outState);
		}

		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			mDrawerToggle.SyncState();
		}

		public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			mDrawerToggle.OnConfigurationChanged(newConfig);
		}
	}
}

