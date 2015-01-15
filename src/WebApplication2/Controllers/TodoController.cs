using System;
using System.Net;
using Microsoft.AspNet.Mvc;
using TodoList.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc.Rendering;
using WebApplication2.Models;

namespace TodoList.Controllers
{
    public class TodoController : Controller
    {
        private readonly ApplicationDbContext db;

        public TodoController(ApplicationDbContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            var todos = db.TodoItems.ToList();

            return View(todos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var todo = await db.TodoItems.SingleOrDefaultAsync(x => x.Id == id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            return View(todo);
        }

        public IActionResult Create()
        {
            ViewBag.Priority = new SelectList(Enumerable.Range(1, 4), "Priority"
                           //  , "pri",Enumerable.Range(1, 4).FirstOrDefault()
                           );
            return View(  );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoItemEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var todo = new TodoItem
            {
                Title = model.Title,
                IsDone = model.IsDone,
                Priority = model.Priority
            };

            db.TodoItems.Add(todo);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
            //return RedirectToAction("Details", new { id = todo.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var todo = await db.TodoItems.SingleOrDefaultAsync(x => x.Id == id);
            if (todo == null)
            {
                return HttpNotFound();
            }

            return View(todo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TodoItemEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var todo = await db.TodoItems.SingleOrDefaultAsync(x => x.Id == id);
            if (todo == null)
            {
                return HttpNotFound();
            }

            todo.Title = model.Title;
            todo.IsDone = model.IsDone;

            // TODO Exception handling
            db.SaveChanges();

            return RedirectToAction("Index", new { id = id });
        }

        // GET: TodoItems/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            TodoItem todoItem = await db.TodoItems.SingleOrDefaultAsync(x => x.Id == id);
            if (todoItem == null)
            {
                return HttpNotFound();
            }
            return View(todoItem);
        }

        // POST: TodoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TodoItem todoItem = await db.TodoItems.SingleOrDefaultAsync(x => x.Id == id);
            db.TodoItems.Remove(todoItem);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
            //return RedirectToAction("Details", new { id = 1 });
        }
    public IActionResult IndexFinal()
    {
      var todos = db.TodoItems.ToList();

      return View(todos);
    }
    protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}