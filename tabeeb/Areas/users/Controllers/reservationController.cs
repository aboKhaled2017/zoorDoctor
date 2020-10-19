using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using tabeeb.Models;
using tabeeb.Controllers;
using tabeeb.Areas.users.languages;
using System.Text.RegularExpressions;
using tabeeb.Areas.users.Models;
namespace tabeeb.Areas.users.Controllers
{
    //this class handle resevation operations when user book appointements on doctors
    public class reservationController : mainController
    {
        //add reservation record to database
        public ActionResult reservation(string name,string phone,string appointementDate,string interval,Guid docID)
        {
            try
            {
                //convert from string to date format
                var appointDate = DateTime.Parse(appointementDate);
                //booked doctor
                var targetDoctor=db.doctors.Find(docID);
                if (!isValidReservingData(phone,name,targetDoctor.bookingType,interval))
                {//reservation data is not valid
                    TempData["error"] =Resource1.invalidBookingData;//show error data on page
                    //redirect to reservation page
                    return RedirectToAction("doctorReservingsPage", new { id = docID });
                }
                //user is not authenticated
                if (!isUserAuthenticated()) return RedirectToAction("doctorReservingsPage", new {id=docID});
                patient currentPatient = getCurrentpatient();
                //number of resrevation that patient already reserved before for the same doctor
                int numberOfreservingAtTheSameDoctorAtTheSameDate = currentPatient.reservings.Count(r=>r.doctorID==docID&&r.reservingDate==appointDate.Date);
                //onlay available for patinet to reserve 2 times at same doctor at same date
                if (numberOfreservingAtTheSameDoctorAtTheSameDate > 2)
                {
                    TempData["error"] = Resource1.maxBookingNumber;
                    return RedirectToAction("doctorReservingsPage", new { id = docID });
                }
                //return if this patient has already booked at this doctor ,with the same date ,with the same patient name
                bool isHasAnybookingForThatPatient =
                    currentPatient.reservings.Any(r => r.doctorID == docID && r.reservingDate == appointDate.Date&&r.patientName==name);
                //user can not book a reservation at the same doctor with the same time with the same patient name
                if (isHasAnybookingForThatPatient)
                {
                    TempData["error"] = Resource1.haveBookingAlready;
                    return RedirectToAction("doctorReservingsPage", new { id = docID });
                }
                //add new reservation record at database
                reserving newReservation = new reserving();
                newReservation.doctorID = docID;
                newReservation.patientName = name;
                newReservation.patientID = currentPatient.id;
                newReservation.reservingDate = appointDate.Date;
                newReservation.visitType = true;
                newReservation.phone = phone;
                newReservation.interval = interval;
                newReservation.id = Guid.NewGuid();
                currentPatient.reservings.Add(newReservation);
                db.SaveChanges();              
                thankForBookingData bookingDoneData=new thankForBookingData();
                bookingDoneData.name=name;
                bookingDoneData.clinicAddress = (currentLanguage == "en") ? targetDoctor.doctorInfo.clinicAddressEng : targetDoctor.doctorInfo.clinicAddressAr;
                bookingDoneData.appointementDate=appointDate;
                bookingDoneData.interval=interval;
                bookingDoneData.bookingType = targetDoctor.bookingType;
                //go to thank you page(booking is done)
                return RedirectToAction("thankYouForBooking", bookingDoneData);
            }
            catch(Exception)
            {
                TempData["error"] = Resource1.serverProblem;
                return RedirectToAction("doctorReservingsPage");
            }            
        }
        //reservation done page
        public ActionResult thankYouForBooking(thankForBookingData data)
        {
            return View(data);
        }
        //page that show appointements for user in order to book his appointements
        public ActionResult doctorReservingsPage(Guid id)
        {
            try
            {
                var doctor= db.doctors.activatedDoctors().Single(d => d.id == id);
                var doctorData =doctor.getDoctorPageInfo(true);
                var appointements = doctor.appointements.Select(a=>new Tuple<Guid,DateTime?,string>(a.id,a.dateOfBooking,a.interval));
                ViewData.Add("appointements",appointements.ToList());
                ViewData.Add("scedualedDayesCount",doctor.doctorInfo.numberOfScedualedDayes);
                return View(doctorData);
            }
            catch
            {
                return Redirect("/" + defaultPathForUsersArea);
            }
            
        }
        //this function check that reservation data is valid 
        private bool isValidReservingData(string phone,string name,bool bookingType,string interval)
        {
            Regex rgxPhone = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{5})$");
            Regex rgxName = new Regex(@"^[a-zA-Z\sأ-ىؤءئآ]{3,30}$");
            if (!(rgxPhone.IsMatch(phone)&&rgxName.IsMatch(name)))
            {
                return false;
            }
            if(bookingType)
            { 
                Regex intervalRgx=new Regex("^{\"from\":\"(\\d\\d:\\d\\d\\s(AM|PM))\",\"to\":\"(\\d\\d:\\d\\d\\s(AM|PM))\"}$");
                if(!intervalRgx.IsMatch(interval))return false;
            }
            else{
                Regex intervalRgx=new Regex("^(\\d\\d:\\d\\d\\s(AM|PM))$");
                if(!intervalRgx.IsMatch(interval))return false;
            }
            return true;
        }
	}
}