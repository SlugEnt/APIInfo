﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugEnt.APIInfo.HealthInfo
{
	public abstract class AbstractHealthChecker : IHealthChecker {
		protected EnumHealthStatus        _status;
		private DateTimeOffset          _lastStatusCheck;
		private DateTimeOffset          _nextStatusCheck;
		private bool                    _isRunning;
		private List<HealthEntryRecord> _healthRecords;


		public AbstractHealthChecker (string name, EnumHealthCheckerType type, IHealthCheckConfig healthCheckConfig) {
			Name = name;
			Config = healthCheckConfig;
			HealthCheckerType = type;
			_status = EnumHealthStatus.Unknown;
			_lastStatusCheck = DateTimeOffset.Now;
			_nextStatusCheck = DateTimeOffset.Now;
			_healthRecords = new List<HealthEntryRecord>();
			_isRunning = false;
		}


		/// <summary>
		/// Name of this specific Health Checker.  Like DB name or Redis Name or something that identifies specifically what it is checking
		/// </summary>
		public string Name { get; set; }


		/// <summary>
		/// The type of thing this Health Checker checks
		/// </summary>
		public EnumHealthCheckerType HealthCheckerType { get ; set; }


		/// <summary>
		/// Configuration object for this HealthChecker
		/// </summary>
		public IHealthCheckConfig Config { get; set; }


		/// <summary>
		/// Current Status of this health checker
		/// </summary>
		public EnumHealthStatus Status {
			get { return _status; }
		}


		/// <summary>
		/// Last time the status was checked, ie, the time the Status entry was last updated
		/// </summary>
		public DateTimeOffset LastStatusCheck {
			get {
				return _lastStatusCheck;}
		}


		/// <summary>
		/// When the next health check for this item should be checked.
		/// </summary>
		public DateTimeOffset NextStatusCheck {
			get { return _nextStatusCheck; }
		}
		

		/// <summary>
		/// List of the last X health checks.  There is an upper limit to how many we keep.
		/// </summary>
		public List<HealthEntryRecord> HealthEntries {
			get { return _healthRecords; }
		}


		/// <summary>
		/// If true the Health Checker is still running and should not be run again.
		/// </summary>
		public bool IsRunning {
			get { return _isRunning; }
		}


		/// <summary>
		/// Name of the Class of Checker
		/// </summary>
		public string CheckerName { get; set; }


		/// <summary>
		/// The maximum allowed number of entries in the HealthEntries list
		/// </summary>
		public int MaxHealthEntries { get; set; } = 100;


		/// <summary>
		/// The maximum age of an entry in the HealthEntries List
		/// </summary>
		public int MaxHealthDays { get; set; } = 375;

		/// <summary>
		/// Executes the Health Check routine for the specific Health Checker (Query database, Write File, etc)  Should return true if everything worked
		/// </summary>
		/// <param name="force"></param>
		/// <returns></returns>
		protected abstract (EnumHealthStatus, string) PerformHealthCheck ();


		/// <summary>
		/// Runs the health check if necessary
		/// </summary>
		/// <param name="force"></param>
		public void CheckHealth (bool force = false) {
			bool needToCheck = force;

			if ( IsRunning ) return;

			// See if we are supposed to run a health check
			if (DateTimeOffset.Now > NextStatusCheck) needToCheck = true;

			if (!needToCheck) return;

			_isRunning = true;

			EnumHealthStatus newStatus;
			string message="";
			(newStatus, message) = PerformHealthCheck();
			if ( newStatus != _status ) {
				HealthEntryRecord healthEntryRecord = new HealthEntryRecord(newStatus, message);
				_healthRecords.Add(healthEntryRecord);
				_status = newStatus;
			}
			else {
				_healthRecords[_healthRecords.Count - 1].Increment();
			}


			// Set next check interval
			_nextStatusCheck = DateTimeOffset.Now.AddSeconds(Config.CheckIntervalSeconds);  


			// See if Age or Capacity limits have been reached on the health records list and remove any that meet criteria.
			if ( _healthRecords.Count > MaxHealthEntries ) {
				while ( _healthRecords.Count > MaxHealthEntries ) {
					_healthRecords.RemoveAt(0);
				}

				DateTimeOffset agingDate = DateTimeOffset.Now.AddDays(-1 * MaxHealthDays);
				int lastIndex = -1;
				bool keepSearching = true;
				int index = 0;
				while ( keepSearching ) {
					if ( _healthRecords [index].LastDateTimeOffset < agingDate ) lastIndex = index;
					break;
				}
				if (lastIndex != -1) _healthRecords.RemoveRange(0,lastIndex);
			}

			_isRunning = false;
		}


		/// <summary>
		/// This method should provide the HTML text that displays the results of this Health Check
		/// </summary>
		/// <returns></returns>
		public abstract void  DisplayHTML (StringBuilder sb);
	}
}