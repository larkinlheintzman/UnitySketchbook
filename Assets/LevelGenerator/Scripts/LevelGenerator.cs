using System.Collections.Generic;
using System.Linq;
using LevelGenerator.Scripts.Exceptions;
using LevelGenerator.Scripts.Helpers;
using LevelGenerator.Scripts.Structure;
// using LevelGenerator.Scripts.LiveEditor;
using UnityEngine;

namespace LevelGenerator.Scripts
{
  public class LevelGenerator : MonoBehaviour
  {

    // LiveEditor editor;
    /// <summary>
    /// LevelGenerator seed
    /// </summary>
    public int Seed;

    /// <summary>
    /// Generate level
    /// </summary>
    public bool isEnabled = true;

    /// <summary>
    /// Container for all sections in hierarchy
    /// </summary>
    public Transform SectionContainer;

    /// <summary>
    /// Maximum level size measured in sections
    /// </summary>
    public int MaxLevelSize;

    /// <summary>
    /// Maximum allowed distance from the original section
    /// </summary>
    public int MaxAllowedOrder;

    /// <summary>
    /// size scaling noise to apply
    /// </summary>
    [SerializeField,Range(0,2)]
    public float ScaleNoise;

    /// <summary>
    /// Spawnable section prefabs
    /// </summary>
    public Section[] Sections;

    /// <summary>
    /// Spawnable dead ends
    /// </summary>
    public DeadEnd[] DeadEnds;

    /// <summary>
    /// Tags that will be taken into consideration when building the first section
    /// </summary>
    public string[] InitialSectionTags;

    /// <summary>
    /// Special section rules, limits and forces the amount of a specific tag
    /// </summary>
    public TagRule[] SpecialRules;

    protected List<Section> registeredSections = new List<Section>();
    protected List<DeadEnd> registeredDeadEnds = new List<DeadEnd>();

    public int LevelSize { get; private set; }
    public Transform Container => SectionContainer != null ? SectionContainer : transform;

    protected IEnumerable<Collider> RegisteredColliders => registeredSections.SelectMany(s => s.Bounds.Colliders).Union(DeadEndColliders);
    protected List<Collider> DeadEndColliders = new List<Collider>();
    protected bool HalfLevelBuilt => registeredSections.Count > LevelSize;

    // private bool updateFlag = true;
    // private bool cleaningFlag = false;

    // Live update modification
    // void OnValidate()
    // {
    //   updateFlag = true;
    // }

    public void GenerateLevel()
    {

      // if (!updateFlag)
      // {
      //   Cleanup();
      // }
      // if (updateFlag)
      // {
      if (Seed != 0)
      {
        RandomService.SetSeed(Seed);
        // Debug.Log("Seed being set to " + Seed);
      }
      else
      {
        Seed = RandomService.Seed;
      }

      CheckRuleIntegrity();
      LevelSize = MaxLevelSize;
      CreateInitialSection();
      DeactivateBounds();
        // updateFlag = false;
      // }

    }

    protected void Start()
    {
      if (Seed != 0)
      RandomService.SetSeed(Seed);
      else
      Seed = RandomService.Seed;

      CheckRuleIntegrity();
      LevelSize = MaxLevelSize;
      CreateInitialSection();
      DeactivateBounds();
      // updateFlag = false;

    }

    public void Cleanup()
    {

      // updateFlag = false;
      // cleaningFlag = true;
      // while (registeredSections.Count > 0)
      // {
        foreach (Section child in registeredSections)
        {
          if (child)
          {
            // Debug.Log("destroying child " + child);
            GameObject.DestroyImmediate(child.gameObject);
          }
        }
        foreach (DeadEnd child in registeredDeadEnds)
        {
          if (child)
          {
            // Debug.Log("destroying child " + child);
            GameObject.DestroyImmediate(child.gameObject);
          }
        }
        // Debug.Log("Container empty iteration, current length: " + registeredSections.Count);
      // }


      // foreach (Collider deadEndCollider in DeadEndColliders)
      // {
      //   GameObject.DestroyImmediate(deadEndCollider.gameObject);
      // }

      // public Transform Container => SectionContainer != null ? SectionContainer : transform;
      registeredSections = new List<Section>();
      // protected IEnumerable<Collider> RegisteredColliders => registeredSections.SelectMany(s => s.Bounds.Colliders).Union(DeadEndColliders);
      DeadEndColliders = new List<Collider>();

      // updateFlag = true;
      // cleaningFlag = false;
    }

    // protected void Update()
    // {
    //
    //   if (updateFlag)
    //   {
    //
    //     Debug.Log("level updated");
    //   }
    //
    // }

    protected void CheckRuleIntegrity()
    {
      foreach (var ruleTag in SpecialRules.Select(r => r.Tag))
      {
        if (SpecialRules.Count(r => r.Tag.Equals(ruleTag)) > 1)
        throw new InvalidRuleDeclarationException();
      }
    }

    protected void CreateInitialSection()
    {
      // Instantiate(PickSectionWithTag(InitialSectionTags), transform).Initialize(this, 0);
      Section newSection = PickSectionWithTag(InitialSectionTags);
      newSection.transform.localScale = Vector3.one;
      Instantiate(newSection, transform).Initialize(this, 0);
      //
      // Transform unscaledTransform = transform;
      // unscaledTransform.localScale = Vector3.one;
      // apply scaling
      //
      // newSection.;
      //
      // newSection.transform.localScale = (Vector3.one * (Random.Range(0.75f, 1.25f)) * ScaleNoise);
      // newSection.transform.localScale.Normalize();
    }

    // public GameObject RandomScale(GameObject source, float minScale, float maxScale)
    // {
    //     GameObject clone = Instantiate(source) as GameObject;
    //     clone.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);
    //     return clone;
    // }


    public void AddSectionTemplate() => Instantiate(Resources.Load("SectionTemplate"), Vector3.zero, Quaternion.identity);
    public void AddDeadEndTemplate() => Instantiate(Resources.Load("DeadEndTemplate"), Vector3.zero, Quaternion.identity);

    public bool IsSectionValid(Bounds newSection, Bounds sectionToIgnore) =>
    !RegisteredColliders.Except(sectionToIgnore.Colliders).Any(c => c.bounds.Intersects(newSection.Colliders.First().bounds));

    public void RegisterNewSection(Section newSection)
    {
      registeredSections.Add(newSection);

      if(SpecialRules.Any(r => newSection.Tags.Contains(r.Tag)))
      SpecialRules.First(r => newSection.Tags.Contains(r.Tag)).PlaceRuleSection();

      LevelSize--;
    }

    public void RegistrerNewDeadEnd(IEnumerable<Collider> colliders, DeadEnd newDeadEnd)
    {
      DeadEndColliders.AddRange(colliders);
      registeredDeadEnds.Add(newDeadEnd);
    }

    public Section PickSectionWithTag(string[] tags)
    {
      if (RulesContainTargetTags(tags) && HalfLevelBuilt)
      {
        foreach (var rule in SpecialRules.Where(r => r.NotSatisfied))
        {
          if (tags.Contains(rule.Tag))
          {
            return Sections.Where(x => x.Tags.Contains(rule.Tag)).PickOne();
          }
        }
      }

      var pickedTag = PickFromExcludedTags(tags);
      return Sections.Where(x => x.Tags.Contains(pickedTag)).PickOne();
    }

    protected string PickFromExcludedTags(string[] tags)
    {
      var tagsToExclude = SpecialRules.Where(r => r.Completed).Select(rs => rs.Tag);
      return tags.Except(tagsToExclude).PickOne();
    }

    protected bool RulesContainTargetTags(string[] tags) => tags.Intersect(SpecialRules.Where(r => r.NotSatisfied).Select(r => r.Tag)).Any();

    protected void DeactivateBounds()
    {
      foreach (var c in RegisteredColliders)
      c.enabled = false;
    }
  }
}
