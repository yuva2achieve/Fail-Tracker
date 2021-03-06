using System.Web.Mvc;
using Microsoft.Web.Mvc;
using FailTracker.Core.Data;
using FailTracker.Core.Domain;
using NHibernate.Linq;
using StructureMap;

namespace FailTracker.Web.Controllers
{
	public class UtilityController : FailTrackerController
	{
		private readonly IContainer _container;

		public UtilityController(IContainer container)
		{
			_container = container;
		}

		public ActionResult Layout()
		{
			return View();
		}

		[HttpGet]
		public ActionResult ResetDatabase()
		{
			return View();
		}

		[HttpPost]
		public ActionResult ResetDatabase(FormCollection form)
		{
			NHibernateBootstrapper.CreateSchema();
			using (var session = NHibernateBootstrapper.GetSession())
			{
				var users = new[]
				            	{
				            		Core.Domain.User.CreateNewUser("admin@failtracker.com", "admin"),
				            		Core.Domain.User.CreateNewUser("user@failtracker.com", "user")
				            	};
				users.ForEach(u => session.Save(u));

				var project = Project.Create("Fail Tracker", users[0]);
				session.Save(project);
				
				(new[] {
				 		Issue.CreateNewIssue(project, "Project support", users[0], "As someone who manages many software projects, I want to be able to organize issues and bugs into projects within Fail Tracker.")
								.ReassignTo(users[0])
								.ChangeSizeTo(PointSize.Eight),
				 		Issue.CreateNewIssue(project, "Site rendering problems in IE6", users[1], "The site does not render the same in al versions of IE!")
								.ChangeTypeTo(IssueType.Bug)
								.ChangeSizeTo(PointSize.OneHundred)
								.ReassignTo(users[1]),
				 		Issue.CreateNewIssue(project, "Enable user invite", users[0], "I want to be able to invite users to join Fail Tracker through a form on the site.")
								.ReassignTo(users[0])
								.ChangeSizeTo(PointSize.Five),
				 		Issue.CreateNewIssue(project, "Support unassigned stories", users[0], "I want to be able to leave stories and bugs unassigned.")
								.ChangeSizeTo(PointSize.Five),
				 	}).ForEach(i => session.Save(i));

				session.Flush();
			}

			return this.RedirectToAction<DashboardController>(c => c.Index());
		}

		public ActionResult Container()
		{
			return View(_container);
		}
	}
}