import { Router, Request, Response } from 'express';
import { body, validationResult } from 'express-validator';
import { renderFormToPdf } from '../utils/renderFormToPDF';
import { getCachedTemplate } from '../services/templateCache';

const router = Router();

router.post(
  '/render',
  body('data').isObject().withMessage('data must be an object'),
  async (req: Request, res: Response): Promise<void> => {
    const errors = validationResult(req);

    if (!errors.isEmpty()) {
      console.error('❌ Validation errors:', errors.array());
      res.status(400).json({
        status: 'error',
        message: 'Validation failed',
        errors: errors.array()
      });
      return;
    }

    try {
      const submissionData = req.body;
      const cachedTemplate = getCachedTemplate();

      if (!cachedTemplate) {
        res.status(503).json({ status: 'error', message: 'Template not ready.' });
        return;
      }

      const pdfBuffer = await renderFormToPdf(submissionData);
      res.set({
        'Content-Type': 'application/pdf',
        'Content-Disposition': 'inline; filename=form.pdf',
      });

      res.send(pdfBuffer);
    } catch (err) {
      console.error('❌ Error rendering PDF:', err);
      res.status(500).json({ status: 'error', message: 'Internal server error.' });
    }
  }
);

export default router;
