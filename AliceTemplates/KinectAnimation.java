package edu.calvin.cs.alicekinect;

import java.util.ArrayList;
import java.util.List;
import org.lgna.story.SBiped;
import org.lgna.story.SScene;

/**
 * Provides an interface for playing animations created with Kinect.
 * @author andrew
 */
public class {0} implements IAnimator {{

	List<IAnimator> animationSegments = new ArrayList<IAnimator>();
	int totalSegments = {1};
	
	public {0}(SScene scene) {{
		try {{
			for (int i = 0; i < totalSegments; i++) {{
				Class segmentClass = Class.forName("edu.calvin.cs.alicekinect.AnimationSegment" + i);
				IAnimator segment = (IAnimator)segmentClass.getDeclaredConstructor(SScene.class).newInstance(scene);
				animationSegments.add(segment);
			}}
		}} catch (Exception e) {{
		}}
	}}

	public void animate(SBiped biped) {{
		for (IAnimator segment : animationSegments) {{
			segment.animate(biped);
		}}
	}}

}}
