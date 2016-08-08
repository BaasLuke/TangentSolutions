
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace TangentSolutions
{
	[Activity(Label = "ProjectDetailsActivity")]
	public class ProjectDetailsFragment : Android.Support.V4.App.Fragment
	{
		static Project project;
		public ProjectDetailsFragment(Project _project)
		{
			Utils utils = new Utils();
			utils.pref = Application.Context.GetSharedPreferences("Session Data", FileCreationMode.Private);
			utils.edit = utils.pref.Edit();

			if (_project != null)
				project = _project;
		}
		public static Android.Support.V4.App.Fragment newInstance(Context context)
		{
			ProjectDetailsFragment busrouteFragment = new ProjectDetailsFragment(project);
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

			ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.ProjectDetails, null);

			EditText txtTitle = root.FindViewById<EditText>(Resource.Id.txtProjectDetailsTitle);
			EditText txtDescription = root.FindViewById<EditText>(Resource.Id.txtProjectDetailsDescription);
			Spinner spnStartDay = root.FindViewById<Spinner>(Resource.Id.spnStartDateDay);
			Spinner spnStartMonth = root.FindViewById<Spinner>(Resource.Id.spnStartDateMonth);
			Spinner spnStartYear = root.FindViewById<Spinner>(Resource.Id.spnStartDateYear);
			Spinner spnEndDay = root.FindViewById<Spinner>(Resource.Id.spnEndDateDay);
			Spinner spnEndMonth = root.FindViewById<Spinner>(Resource.Id.spnEndDateMonth);
			Spinner spnEndMYear = root.FindViewById<Spinner>(Resource.Id.spnEndDateYear);
			CheckBox chkIsActive = root.FindViewById<CheckBox>(Resource.Id.chkIsActive);
			CheckBox chkIsBillable = root.FindViewById<CheckBox>(Resource.Id.chkIsBillable);
			Button btnUpdate = root.FindViewById<Button>(Resource.Id.btnProjectDetailsUpdate);
			Button btnDelete = root.FindViewById<Button>(Resource.Id.btnProjectDetailsDelete);

			List<String> startDayArray = new List<String>();

			for (int x = 1; x <= 31; x++)
			{
				if (x < 10)
					startDayArray.Add("0" + x);
				else
					startDayArray.Add(x.ToString());
			}

			List<String> startMonthArray = new List<String>();

			for (int x = 1; x <= 12; x++)
			{
				if (x < 10)
					startMonthArray.Add("0" + x);
				else
					startMonthArray.Add(x.ToString());
			}

			List<String> startYearArray = new List<String>();

			for (int x = 2015; x < 2020; x++)
			{
				if (x < 10)
					startYearArray.Add("0" + x);
				else
					startYearArray.Add(x.ToString());
			}

			List<String> endDayArray = new List<String>();

			for (int x = 1; x <= 31; x++)
			{
				if (x < 10)
					endDayArray.Add("0" + x);
				else
					endDayArray.Add(x.ToString());
			}

			List<String> endMonthArray = new List<String>();

			for (int x = 1; x <= 12; x++)
			{
				if (x < 10)
					endMonthArray.Add("0" + x);
				else
					endMonthArray.Add(x.ToString());
			}

			List<String> endYearArray = new List<String>();

			for (int x = 2015; x < 2020; x++)
			{
				if (x < 10)
					endYearArray.Add("0" + x);
				else
					endYearArray.Add(x.ToString());
			}

			ArrayAdapter startDayAdapter = new ArrayAdapter (root.Context, Android.Resource.Layout.SimpleSpinnerItem, startDayArray);
			startDayAdapter.SetDropDownViewResource (Resource.Layout.dropdown_item);
			spnStartDay.Adapter = startDayAdapter;

			ArrayAdapter startMonthAdapter = new ArrayAdapter (root.Context, Android.Resource.Layout.SimpleSpinnerItem, startMonthArray);
			startMonthAdapter.SetDropDownViewResource (Resource.Layout.dropdown_item);
			spnStartMonth.Adapter = startMonthAdapter;

			ArrayAdapter startYearAdapter = new ArrayAdapter (root.Context, Android.Resource.Layout.SimpleSpinnerItem, startYearArray);
			startYearAdapter.SetDropDownViewResource (Resource.Layout.dropdown_item);
			spnStartYear.Adapter = startYearAdapter;

			ArrayAdapter endDayAdapter = new ArrayAdapter (root.Context, Android.Resource.Layout.SimpleSpinnerItem, endDayArray);
			endDayAdapter.SetDropDownViewResource (Resource.Layout.dropdown_item);
			spnEndDay.Adapter = endDayAdapter;

			ArrayAdapter endMonthAdapter = new ArrayAdapter (root.Context, Android.Resource.Layout.SimpleSpinnerItem, endMonthArray);
			endMonthAdapter.SetDropDownViewResource (Resource.Layout.dropdown_item);
			spnEndMonth.Adapter = endMonthAdapter;

			ArrayAdapter endYearAdapter = new ArrayAdapter (root.Context, Android.Resource.Layout.SimpleSpinnerItem, endYearArray);
			endYearAdapter.SetDropDownViewResource (Resource.Layout.dropdown_item);
			spnEndMYear.Adapter = endYearAdapter;

			if (utils.pref.GetBoolean("IsCreate", false) == false)
			{
				try
				{
					txtTitle.Text = project.Title;
					txtDescription.Text = project.Description;
					chkIsActive.Checked = project.IsActive;
					chkIsBillable.Checked = project.IsBillable;
					int spinnerPosition = startDayAdapter.GetPosition(Convert.ToDateTime(project.StartDate).Day.ToString());
					spnStartDay.SetSelection(spinnerPosition);
					spinnerPosition = startMonthAdapter.GetPosition(Convert.ToDateTime(project.StartDate).Month.ToString());
					spnStartMonth.SetSelection(spinnerPosition);
					spinnerPosition = startYearAdapter.GetPosition(Convert.ToDateTime(project.StartDate).Year.ToString());
					spnStartYear.SetSelection(spinnerPosition);
					spinnerPosition = endDayAdapter.GetPosition(Convert.ToDateTime(project.EndDate).Day.ToString());
					spnEndDay.SetSelection(spinnerPosition);
					spinnerPosition = endMonthAdapter.GetPosition(Convert.ToDateTime(project.EndDate).Month.ToString());
					spnEndMonth.SetSelection(spinnerPosition);
					spinnerPosition = endYearAdapter.GetPosition(Convert.ToDateTime(project.EndDate).Year.ToString());
					spnEndMYear.SetSelection(spinnerPosition);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}

			}
			else
			{
				btnDelete.Visibility = ViewStates.Gone;
				btnUpdate.Text = "Create";
			}

			btnUpdate.Click += (sender, e) =>
			{
				if (string.IsNullOrEmpty(txtTitle.Text))
				{
					txtTitle.SetError("Required field", null);
					txtTitle.RequestFocus();
					return;
				}

				if (string.IsNullOrEmpty(txtDescription.Text))
				{
					txtDescription.SetError("Required field", null);
					txtDescription.RequestFocus();
					return;
				}

				if (utils.pref.GetBoolean("IsCreate", false) == false)
				{
					utils.edit.PutInt("PK", project.PK);
					utils.edit.PutString("Action", "Update").Commit();
				}
				else
				{
					utils.edit.PutString("Action", "Create").Commit();
				}

				utils.edit.PutString("Title", txtTitle.Text);
				utils.edit.PutString("Description", txtDescription.Text);
				utils.edit.PutBoolean("IsActive", chkIsActive.Checked);
				utils.edit.PutBoolean("IsBillable", chkIsBillable.Checked);
				utils.edit.PutString("StartDate", spnStartYear.SelectedItem.ToString() + "-" + spnStartMonth.SelectedItem.ToString() + "-" + spnStartDay.SelectedItem.ToString());
				utils.edit.PutString("EndDate", spnEndMYear.SelectedItem.ToString() + "-" + spnEndMonth.SelectedItem.ToString() + "-" + spnEndDay.SelectedItem.ToString()).Commit();
				new doThreadDetails(root.Context).Execute();
			};

			btnDelete.Click += (sender, e) =>
			{
				AlertDialog alert = new AlertDialog.Builder(root.Context).Create();
				alert.SetCancelable(false);
				alert.SetTitle("TANGENT SOLUTIONS");
				alert.SetMessage("Are you sure you want to Delete this Project?");
				alert.SetButton("No", (Nsender, Ne) =>
					{
						alert.Dismiss();
					});
				alert.SetButton2("Yes", (Ysender, Ye) =>
				{
					alert.Dismiss();
					utils.edit.PutInt("PK", project.PK);
					utils.edit.PutString("Title", txtTitle.Text);
					utils.edit.PutString("Description", txtDescription.Text);
					utils.edit.PutBoolean("IsActive", chkIsActive.Checked);
					utils.edit.PutBoolean("IsBillable", chkIsBillable.Checked);
					utils.edit.PutString("StartDate", spnStartYear.SelectedItem.ToString() + "-" + spnStartMonth.SelectedItem.ToString() + "-" + spnStartDay.SelectedItem.ToString());
					utils.edit.PutString("EndDate", spnEndMYear.SelectedItem.ToString() + "-" + spnEndMonth.SelectedItem.ToString() + "-" + spnEndDay.SelectedItem.ToString()).Commit();
					new doThreadDetails(root.Context).Execute();
				});
				alert.Show();
			};

			return root;
		}

		public void DisplayError(Context context, string errorMessage)
		{
			AlertDialog alert = new AlertDialog.Builder(context).Create();
			alert.SetCancelable(false);
			alert.SetTitle("TANGENT SOLUTIONS");
			alert.SetMessage(errorMessage);
			alert.SetButton("OK", (sender, e) =>
				{
					alert.Dismiss();
				});
			alert.Show();
		}

		private class doThreadDetails : AsyncTask<String, String, String>
		{
			Context context;
			ProgressDialog pdIntro = null;
			protected ProjectDetailsFragment activity;
			List<Project> projects = new List<Project>();
			public doThreadDetails(Context context)
			{
				this.context = context;
				this.activity = new ProjectDetailsFragment(project);
			}

			protected override void OnPreExecute()
			{
				base.OnPreExecute();

				pdIntro = new ProgressDialog(context); 
				pdIntro.SetCancelable(false);
				pdIntro.Indeterminate = true;
				pdIntro.SetMessage("Please Wait...");
				pdIntro.SetProgressStyle(ProgressDialogStyle.Spinner);
				pdIntro.SetTitle("Processing");
				pdIntro.Show();

			}

			protected void PublishProgress(params Java.Lang.Object[] values)
			{
				base.PublishProgress(values);
			}

			protected override void OnProgressUpdate(params Java.Lang.Object[] values)
			{
				pdIntro.SetMessage(values[0].ToString());
			}

			protected override void OnPostExecute(Java.Lang.Object @params)
			{
				base.OnPostExecute(@params);

				Utils utils = new Utils();
				utils.pref = context.GetSharedPreferences("Session Data", FileCreationMode.Private);
				utils.edit = utils.pref.Edit();

				pdIntro.Hide();
				pdIntro.Dismiss();
				pdIntro.Dispose();

				if (utils.pref.GetBoolean("HasError", false) == true)
				{
					activity.DisplayError(context, utils.pref.GetString("ErrorMessage", ""));
					utils.edit.PutBoolean("HasError", false);
					utils.edit.PutString("ErrorMessage", "");
					utils.edit.Commit();
				}
			}

			protected override string RunInBackground(params string[] @params)
			{
				Utils utils = new Utils();
				utils.pref = context.GetSharedPreferences("Session Data", FileCreationMode.Private);
				utils.edit = utils.pref.Edit();

				try
				{
					RestRequest request = null;
					Project newProject = new Project();
					switch (utils.pref.GetString("Action", null))
					{
						case "Create":
							request = new RestRequest(Method.POST);
							request.AddHeader("postman-token", "b2c05cb3-332f-3f6f-6b26-c3a52f1d7863");
							break;
						case "Update":
							request = new RestRequest(Method.PUT);
							request.AddHeader("postman-token", "8d5904fd-e695-e64e-ee46-d87dcd53d014");
							newProject.PK = utils.pref.GetInt("PK", 0);
							break;
						case "Delete":
							request = new RestRequest(Method.DELETE);
							request.AddHeader("postman-token", "1dd51584-4b69-ba28-a080-27371fb10931");
							newProject.PK = utils.pref.GetInt("PK", 0);
							break;
					}


					newProject.Description = utils.pref.GetString("Description", null);
					newProject.Title = utils.pref.GetString("Title", null);
					newProject.EndDate = Convert.ToDateTime(utils.pref.GetString("EndDate", "0000-00-00"));
					newProject.StartDate = Convert.ToDateTime(utils.pref.GetString("StartDate", "0000-00-00"));
					newProject.IsActive = utils.pref.GetBoolean("IsActive", false);
					newProject.IsBillable = utils.pref.GetBoolean("IsBillable", false);
					newProject.Resources = null;
					newProject.Tasks = null;

					var client = new RestClient("http://projectservice.staging.tangentmicroservices.com/api%C2%ADexplorer/");
					request.AddHeader("cache-control", "no-cache");
					request.AddHeader("authorization", "Token " + utils.pref.GetString("Token", null));
					request.AddHeader("content-type", "application/json");

					request.AddObject(newProject);

					IRestResponse response = client.Execute(request);

					var result = response.Content;

					utils.edit.PutString("ErrorMessage", utils.pref.GetString("Action", null) + " processed successfully");
					utils.edit.PutBoolean("HasError", true).Commit();
				}
				catch (Exception ex)
				{
					utils.edit.PutBoolean("HasError", true);
					utils.edit.PutString("ErrorMessage", ex.Message).Commit();
				}

				return "";
			}
		}
	}
}

