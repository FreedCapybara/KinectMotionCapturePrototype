package edu.calvin.cs.alicekinect;

import java.util.ArrayList;
import java.util.List;
import org.lgna.story.SBiped;
import org.lgna.story.SScene;

/**
 * Provides an interface for playing animations created with Kinect.
 * @author andrew
 */
public class KinectAnimation implements IAnimator {{

	List<IAnimator> animationSegments = new ArrayList<IAnimator>();
	int totalSegments = {0};
	
	public KinectAnimation(SScene scene) throws Exception {{
		for (int i = 0; i < totalSegments; i++) {{
			Class segmentClass = Class.forName("edu.calvin.cs.alicekinect.AnimationSegment" + i);
			IAnimator segment = (IAnimator)segmentClass.newInstance();
			animationSegments.add(segment);
		}}
	}}

	public void animate(SBiped biped) {{
		for (IAnimator segment : animationSegments) {{
			segment.animate(biped);
		}}
	}}

}}
