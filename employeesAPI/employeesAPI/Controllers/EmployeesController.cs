﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using employeesAPI.Models;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace employeesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        EmployeeDB employeeDB = new EmployeeDB();

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Employee>> Get(string token)
        {
            if(RSAClass.Decrypt(token) =="token")
                return employeeDB.getAllEmployee().ToList();
            return BadRequest();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Employee> Get(int id, string token)
        {
            if (RSAClass.Decrypt(token) == "token")
                return employeeDB.getEmployeeByID(id);
            return BadRequest();
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody] Employee emp, string token)
        {
            if (RSAClass.Decrypt(token) == "token")
                employeeDB.addEmployee(emp);
            return BadRequest();
        }

        // PUT api/values/5
        [HttpPut]
        public ActionResult Put([FromBody] Employee emp, string token)
        {
            if (RSAClass.Decrypt(token) == "token")
                employeeDB.updateEmployee(emp);
            return BadRequest();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id, string token)
        {
            if (RSAClass.Decrypt(token) == "token")
                employeeDB.deleteEmployee(id);
            return BadRequest();
        }

        /*EmployeeDB employeeDB = new EmployeeDB();
        [HttpGet("all")]
        public ActionResult<IEnumerable<Employee>> Index([FromQuery] string data)
        {
            if (RSAClass.Decrypt(data) == "all")
                return employeeDB.getAllEmployee().ToList();
            return NotFound();
        }

        [HttpGet("emp")]
        public Employee SingleEmployee([FromQuery] string data)
        {
            int id = Int16.Parse(RSAClass.Decrypt(data));
            return employeeDB.getEmployeeByID(id);
        }

        [HttpGet("create")]
        public IActionResult Create([FromQuery] string name, [FromQuery] string gender, [FromQuery] string department, [FromQuery] string username, [FromQuery] string password)
        {
            Employee emp = new Employee();
            emp.Name = name;
            emp.Gender = gender;
            emp.Department = department;
            emp.Username = username;
            emp.Password = password;
            employeeDB.addEmployee(emp);
            return Accepted();
        }

        [HttpGet("edit")]
        public IActionResult Edit([FromQuery] int id, [FromQuery] string name, [FromQuery] string gender, [FromQuery] string department, [FromQuery] string username, [FromQuery] string password)
        {
            if(employeeDB.getEmployeeByID(id)==null)
                return NotFound();
            Employee emp = new Employee();
            emp.ID = id;
            emp.Name = name;
            emp.Gender = gender;
            emp.Department = department;
            emp.Username = username;
            emp.Password = password;
            employeeDB.updateEmployee(emp);
            return Accepted();
        }

        [HttpGet("delete")]
        public IActionResult Delete([FromQuery] string data)
        {
            int id = Int16.Parse(RSAClass.Decrypt(data));
            if (employeeDB.getEmployeeByID(id) == null)
                return NotFound();
            employeeDB.deleteEmployee(id);
            return Accepted();
        }*/
    }
}