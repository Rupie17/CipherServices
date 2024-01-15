using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CipherServices.Services;
using CipherServices.Data;
using CipherServices.Models;

namespace CipherServices.Pages
{
  public class IndexModel : PageModel
  {
    public Dictionary<string, string> Secrets { get; set; }
    [BindProperty]
    public Message NewMessage { get; set; }

    private readonly MessageContext _context;
    private readonly IDecrypter _decrypter;
    private readonly IEncrypter _encrypter;

    public IndexModel(MessageContext context, IDecrypter decrypter, IEncrypter encrypter)
    {
      _context = context;
      _decrypter = decrypter;
      _encrypter = encrypter;
    }

    public async Task<IActionResult> OnGetAsync()
    {
      await LoadSecretsAsync(_decrypter, _context);
      
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (ModelState.IsValid) {
        string cleanedText = NewMessage.Text.Trim().ToLower();
        string encryptedText = _encrypter.Encrypt(cleanedText);
        Message m = new Message{Text = encryptedText};
        _context.Messages.Add(m);
        await _context.SaveChangesAsync();
        return RedirectToPage("/Index");
      } else {
        await LoadSecretsAsync(_decrypter, _context);
        return Page();
      }

    }


    private async Task LoadSecretsAsync(IDecrypter decrypter, MessageContext context)
    {
      Secrets = new Dictionary<string, string>();
      var messages = await context.Messages.ToListAsync();

      foreach (Message m in messages)
      {
        Secrets.TryAdd(m.Text, decrypter.Decrypt(m.Text));
      }
    }
  }
}
