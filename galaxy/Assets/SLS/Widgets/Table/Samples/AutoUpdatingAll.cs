using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SLS.Widgets.Table {
public class AutoUpdatingAll : MonoBehaviour {

  public Table table;
  public Sprite sprite1;
  public Sprite sprite2;
  public Sprite sprite3;
  public Sprite sprite4;
  public Sprite sprite5;

  private Dictionary<string, Sprite> spriteDict;
  private List<string> spriteNames;

  void Start () {

    this.spriteDict = new Dictionary<string, Sprite>();
    this.spriteDict.Add("1", this.sprite1);
    this.spriteDict.Add("2", this.sprite2);
    this.spriteDict.Add("3", this.sprite3);
    this.spriteDict.Add("4", this.sprite4);
    this.spriteDict.Add("5", this.sprite5);

    this.spriteNames = new List<string>(this.spriteDict.Keys);

    this.table.reset();

    this.table.addImageColumn("h0", "f0", 32, 32);
    this.table.addTextColumn("h1", "f1");
    this.table.addTextColumn("h2", "f2");
    this.table.addImageColumn("Wide Column Name", "f3", 32, 32);
    this.table.addTextColumn("h4", "f4");
    this.table.addTextColumn("h5", "f5");
    this.table.addImageColumn("h6", "f6", 32, 32);
    this.table.addTextColumn("h7", "f7", -1, 400);

    this.table.initialize(this.onRowSelected, this.spriteDict);

    // Populate Your Rows (obviously this would be real data here)
    for (int i = 0; i < 5; i++) {
      Datum d = this.makeDatum("INIT_" + i.ToString());
      d.uid = i.ToString();
      this.table.data.Add(d);
    }

    // Draw Your Table
    this.table.startRenderEngine();

    //StartCoroutine(this.doRandomData());

  }

  private Datum makeDatum(string pfx) {
    string sfx = Time.realtimeSinceStartup.ToString();
    Datum d = Datum.Body(sfx);
    d.elements.Add(this.randomSprite());
    d.elements.Add("Col1:" + pfx + ":" + sfx);
    d.elements.Add("Col2:" + pfx + ":" + sfx);
    d.elements.Add(this.randomSprite());
    d.elements.Add("Col4:" + pfx + ":" + sfx);
    d.elements.Add("Col5:" + pfx + ":" + sfx);
    d.elements.Add(this.randomSprite());
    string c7 = "Col7:" + pfx + ":" + sfx;
    int words = Random.Range(0, 40);
    for (int i = 0; i < words; i++) {
      c7 += ": WORD" + i.ToString();
    }
    d.elements.Add(c7);
    return d;
  }

  private string randomSprite(){
    int idx = Random.Range(0, this.spriteNames.Count);
    return this.spriteNames[idx];
  }

  IEnumerator doRandomData() {
    yield return new WaitForSeconds(2f);
    while (true) {
      float action = Random.Range(0, 50);
      if (action < 5) {
        this.table.data.Add(this.makeDatum("ADD"));
      }
      else if (action < 10) {
        this.pushRowTop();
      }
      else if (action < 13) {
        this.pushRowRandom();
      }
      else if (action < 16) {
        this.pushRowBottom();
      }
      else if (action < 20) {
        deleteRow();
      }
      else if (action < 25) {
        int cidx = Random.Range(0, this.table.columns.Count);
        string x = "UPD:" + Time.realtimeSinceStartup.ToString();
        for (int i = 1; i < Random.Range(0, 10); i++) {
          x = x + "\nLine:" + i.ToString();
        }
        this.table.columns[cidx].headerValue = x;
      }
      else if (action < 30) {
        int cidx = Random.Range(0, this.table.columns.Count);
        string x = "UPD:" + Time.realtimeSinceStartup.ToString();
        for (int i = 1; i < Random.Range(0, 4); i++) {
          x = x + "\nLine:" + i.ToString();
        }
        this.table.columns[cidx].footerValue = x;
      }
      else {
        this.updateRow();
      }
      yield return new WaitForSeconds(Random.Range(0.5f, 1f));
      //yield return new WaitForSeconds(0.5f);
    }
  }

  // Handle the Row Selection however you wish
  private void onRowSelected(Datum datum) {
    print("You Clicked: " + datum.uid + " with safe height: " + datum.safeHeight());
    for (int i = 0; i < datum.elements.Count; i++) {
      print(datum.elements[i].value);
    }
  }

  // Handle Header Selection however you wish
  private void onHeaderSelected(Column column) {
    print("You Clicked Column: " + column.idx + " " + column.headerValue);
  }

  public void pushRowTop() {
    this.table.data.Insert(0, this.makeDatum("TOP"));
  }

  public void pushRowRandom() {
    int ridx = Random.Range(0, this.table.data.Count);
    this.table.data.Insert(ridx, this.makeDatum("INS"));
  }

  public void pushRowBottom() {
    this.table.data.Insert(this.table.data.Count, this.makeDatum("BOT"));
  }

  public void deleteRow() {
    if (this.table.data.Count == 0) return;
    int ridx = Random.Range(0, this.table.data.Count);
    this.table.data.RemoveAt(ridx);
  }

  public void updateRow() {
    int ridx = Random.Range(0, this.table.data.Count);
    int cidx = Random.Range(0, this.table.columns.Count);
    //print("Updating Row: " + ridx + " Column: " + cidx);
    if (cidx != 0 && cidx != 3 && cidx != 6) {
      string x = "UPD:" + Time.realtimeSinceStartup.ToString();
      for (int i = 1; i < Random.Range(0, 20); i++) {
        x = x + "\nLine:" + i.ToString();
      }
      this.table.data[ridx].elements[cidx].value = x;
    }
    else
      this.table.data[ridx].elements[cidx].value =
        this.randomSprite();
  }

}
}