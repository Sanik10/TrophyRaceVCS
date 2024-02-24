THANKS for your support!

Thank you for purchasing Castle Valley Collection #1!  These rock formations were scanned in the San Rafael Swell, a heavily eroded upwelling of Jurrasic sandstone in central Utah, not far from Moab.  Thanks goes out to the folks at 3DF Zephyr and 3D Coat and Unity for their great software.  Thanks also goes to my pack goats Shelby GT, Woodstock, Bacchus, Vincent VanGoat, Barry Goatalo, and also Luna the German Shep, for their assistance carrying equipment out in the desert.  If Collection #1 is successful, Collection #2 will be complimentary and compatible, containing more of the most useful building blocks for amazing scenery.  

----------------------------

SUPPORT

Web Page:    http://www.goatogrammetry.com/
Email:  goatogrammetry@gmail.com

----------------------------

SETUP

This version of Castle Valley Collection #1 is meant for Unity's "High Definition Render Pipeline" HDRP.  Because there are no scripts or settings that will override or interfere with any of your existing work, it should be safe to install it directly into your current project.

* Be sure your HDRP scene is properly configured.  You'll need to make sure you're not using the default HDRP Asset, as that one is in the packages directory and can not be modified.  If you haven't already, create a new one (Assets/Create/Rendering/High Definition Render Pipeline Asset) and put it in your settings folder, or somewhere convenient.  You will need to edit it because of the following...

* HDRP translucent materials require that their 'diffusion profiles' be added to your HDRP Pipeline Asset or the plant models using them will look CANDY GREEN.  The profile is in Materials/Shaders and is called "Sage Diffusion Profile".  Locate your HDRP asset in Project Settings/Graphics at the top of the menu.  Find the diffusion profiles section and add the Sage diffusion profile.

A scene in the Demo directory "Castle Valley Layout Scene" will show all of the prefabs and tiling textures in this pack lined up in rows.  In order to keep the size down and not clutter your project its very basic, with no baked lighting, no baked reflection probes, no camera scripts, and no skybox.  Feel free to delete it when you've finished looking it over.

Thats it for setup!

-----------------------------

TIPS:

* PBR shaders require reflection probes to work.  If you don't have probes added (and also baked), your cliffs and rocks will look too glossy or plastic-like as they reflect the skybox rather than what is actually around them.  Often the skybox has bright colors even on the lower half, which will reflect upwards onto the undersides of shadowy overhangs, giving weird "plastic" looking results.  If you drop some cliffs from this pack into a new, empty scene, and the shadows are harsh and very black, its because there is no ambient lighting at all.  Once the scene has correct lighting, you'll be pleasantly surprised at the improved realism of these scanned assets.

* Some of the tiniest rocks and plants get their ambient lighting from light probes rather than baked lightmaps.  If you do not have your light probes set up,  you may find these prefabs are rendering too dark or bright.  When you add light probes, be sure to place them near any prefabs that need them, especially if there is a strong shadow boundary in the area. 

* Keep in mind that prefabs that use the "Cliff" prefix are very large and look best with a little distance, even though they use a 2nd 'detail texture' to enhance the micro-surface.  Because of their large size, their pixel density is lower than the other, smaller prefabs.  They look fine up close, but due to their scale there's no reasonable way to make them look quite as detailed as some of the other, smaller items when the camera is very close to the surface.

-----------------------------

LOD (Level Of Detail) or "LOD Group" component:

* All meshes come with 4 or more hand-adjusted LODs built in.  

* As a rule, LOD1 has half the triangle count as LOD0, etc.  

* Remember that you can adjust global LOD distance ratio in your project settings (LOD bias).

* Special Note:  As of this writing (Unity 2019.3b) Progressive GPU (Not CPU) light baker malfunctions on meshes with LODs, causing blurry, blotchy bakes with glowing and missing black zones.  There's nothing wrong with these meshes-- Its just that Progressive GPU is still in preview beta.  I suggest Bakery GPU Lightmapper (Unity Asset Store) for great results, or using Progressive GPU only for rough-draft light baking, and using the slower CPU version for the final bake.  If you use Progressive CPU, I suggest using 5 or 6 texels for indirect, and dropping the samples from 100 to 16 or so and trusting the de-noising routines to smooth it out.  These rocks are forgiving and imperfections/low resolution in the indirect lighting looks fine.

-----------------------------

COLLIDERS

* All prefabs contain custom made low poly but accurate convex mesh colliders.

* The colliders are not contained within the actual rock's mesh file, but rather in Meshes/Colliders.

* The physics material for Sandstone is in Meshes/Colliders.  I just guessed on the values, so adjust as you see fit.

-----------------------------

MATERIALS AND TEXTURES

* Sometimes different meshes share the same material's bitmap for optimization.

* Mesh filenames have a naming convention that links them to their matching material.  

* An example:  Mesh_Boulder_01_AJ.FBX uses Material_AJ_Main_01.mat which gets its colors from AJ_Color_01.png (Note the AJ in each)

* Color and Normal Maps are generally 4K, but since the Mask Map's detail level is less critical for rock, it is set to 2K or less in the import to save video memory.  I believe you could drop this down to 1K and get away with it.  The source files are provided in 4K, however.

* In HDRP the Mask Map contains:  Red=Metal / Green=Ambient Occlusion / Blue=Detail Texture Mask / Alpha = Roughness

* Some materials have a "Layered" version that allows better blending to the terrain etc.  Change and customize the layer textures as you please to add sand between rocks, different dirt, grass, etc.

* HDRP translucent materials require that their 'diffusion profiles' be added to your HDRP Pipeline Asset or the plant models using them will look CANDY GREEN.  The profile is in Materials/Shaders and is called "Sage Diffusion Profile".  Locate your HDRP asset in Project Settings/Graphics at the top of the menu.  Find the diffusion profiles section and add the Sage diffusion profile.  Because the default "HDRP Asset" is not editable, you'll need to make your own or the changes wont save.  See the "Setup" section above.

-----------------------------

LIGHT MAPS

* Any meshes that share the same texture have a 2nd UV set for light maps.  This 2nd UV set fills the whole UV space to prevent wasted texels in the lightmaps.

* Special Note:  As of this writing (Unity 2019.3b) Progressive GPU (Not CPU) light baker malfunctions on meshes with LODs, causing blurry, blotchy bakes with glowing and missing black zones.  There's nothing wrong with these meshes-- Its just that Progressive GPU is still in preview beta.  I suggest Bakery GPU Lightmapper (Unity Asset Store) for great results, or using Progressive GPU only for rough-draft light baking, and using the slower CPU version for the final bake.  If you use Progressive CPU, I suggest using 5 or 6 texels for indirect, and dropping the samples from 100 to 16 or so and trusting the de-noising routines to smooth it out.  These rocks are forgiving and imperfections/low resolution in the indirect lighting looks fine.

* Some of the tiniest rocks, plants, and the tree are set to take their lighting from the light probes rather than light maps.  Its easy to change, but they're set this way to prevent crazy-long bake times and huge lightmap data.

* All of the "Cliff" prefabs have their light mapping scale set to 1/3 normal texel density due to their large size.  Because you're dealing with large meshes, its safe to reduce this to even smaller fractions and still have a great looking final light-bake!

* "Cliff" prefabs often have "Cap" meshes on the top and back.  These do not bake light maps and use light probes instead.

* "Cap" meshes on cliffs are meant to help cast shadows but are not really made to be seen.  Hide them with capstones etc if necessary.

* Sometimes the cliff's cap meshes will throw warnings from the light baker, saying they have overlapping UVs.  Ignore this, since the UV size is set to 0 (not meant to be seen) and is thus ignored by the indirect light baking algorithm entirely.  

-----------------------------

TERRAIN

* As of this writing, the "terrain detail" texture for HDRP is broken.  Anything you add as a detail terrain mesh will draw with the white default texture.  I have included 'detail rocks' regardless and hopefully in the future they work fine.  For now the sage brush can be painted onto terrain if you consider it a tree.

* A terrain texture is included (Materials/Tiling dir). If you make your own, be sure to set it to allow height blending for best results.  The HDRP terrain texture uses the blue channel in the material's mask.png to 'rough up' the smooth blending between texture layers.  Its a neat feature but most people won't figure out how to set it up correctly. 

* When adding "Terrain Layers" (materials) to paint on the terrain, you'll find them in the Materials/Tiling directory.

-----------------------------

DECALS

* HDRP decal materials are in Materials/Decals...  Just a few mossy blotchy weed patches and some erosion cuts.

* Create decal projectors in the GameObject/Rendering dropdown.

-----------------------------

Change Log

-----------------------------

Version 1.1

All prefabs with problems with the UV2 (baked lighting) problems are fixed.  But since UVs got changed, you'll have to re-bake your maps :(  Sorry!

By popular demand, I added an example scene (sort of a PVP deathmatch map) found in the demo directory.  I suggest adding the simple camera controller script that comes with HDRP so you can move around in play mode.  Progressive lightmapper choked and died so I used Bakery.

The sage brush and trees have been changed to NOT be static.  I found out static terrain trees are a no-no.  All plants will take their lighting from probes.  Also Sage's most distant LOD3 no longer casts shadows just for optimization's sake.

I added a version of the sage brush with the vertex normals pointing upward.  This evens out the shading on the branches.  It may or may not be an improvement for your scenes, so try it out!

Cliff_Corner_03's prefab somehow had the mesh offset from the colliders by .4 meters.  Its fixed, but you'll want to make sure it didnt introduce a gap in your own maps.  If so, just move it a bit.

Obstacle 10-13's albedo texture was slightly too bright and didn't match the rest perfectly.  Its been adjusted.

I've turned off the vulture script's debug message flag.  I left it on originally-- Annoying!

Baked occlusion sliders for the ledge and rock pile materials has been lowered slightly-- It was too intense originally.

Special Note:  If you're converting from the Standard 1.0 to Standard 1.1+, or to HDRP or pretty much any combination, you may have to change the meta ID# for the vulture, obstacle1, rabbit brush, and living tree prefabs to match.  I had to rebuild them from scratch and I forgot to paste-in the right prefab ID, and now people have been using the new patch, so its kinda out of hand.  Sorry about that!

-----------------------------

Version 1.2

Owners of Castle Valley Collection #1 (HDRP) can download the Standard Pipeline version for free!

The pack is now using the 2020.3 LTS version of Unity

The plant leaf shader is now working again-- It broke at some point when Unity updated something.

Many of the color textures were sharpened slightly with Art Engine.  Baked textures always tend to 'average out' a bit, and the AI sharpening did a great job of restoring the original crispness.  

Many of the most important cliff pieces got big improvements to their LODs.  This should reduce LOD popping significantly!  But it does cost a bit of VRam and a few extra state changes because this involved making a new, smaller normal map for the lower LODs along with a new material.  This corrects for the changes in geometry and keeps the shading consistent.  (To go back to single material for all LODs, you can just keep the old prefab objects.)

Some of the cliffs got work done on the normals to sharpen edges.

Several new "detail textures" were added.  This time they were done using correct photogrammetry surface-scans and tiled with Art Engine rather than a simple color-to-hight filter like the original one.  The big cliffs use one of the new ones-- Check out the realistic scalloped shatter patterns on the big flat slabs of sandstone.  Also some of the mask-maps are now being used to prevent the detail textures from revealing the seams.  A nice improvement!

Some of the light-map scales have been changed to more appropriate values.  Sorry-- That means you should probably re-bake the lighting... Though honestly I think Screen Space Global Illumination is the future.

In testing I became aware that prefabs that you flip by scaling by a negative value look fine in the 'scene' and 'game' window, but in 'play' mode the normals appear to be reversed.  I believe this is a Unity bug and not a problem with the assets.  Just be aware.

Remember to leave a review!  Thanks!!!

---------------------------------

Version 1.5

Many of the rock formations have new versions that have been distorted to have more slope, taper, or other useful shapes.  For instance, some pillars now have curved versions that make them useful for creating arches.  Cliffs now have sloped versions that let you move away from purely vertical sandstone ledges.  You'll love these new shapes!  See them all in the layout scene in the "Demo" directory.